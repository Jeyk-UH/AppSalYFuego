using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServicePedido : IServicePedido
    {
        private const int TopProductosMasVendidos = 5;

        // IdEstado y IdEstacion (tablas ESTADO_PEDIDO/ESTACION, ver seed en SalYFuegoDB_Maestro.sql):
        // el pedido ya se cobró en caja, así que entra directo como Aceptada, rumbo a Cocina.
        private const int IdEstadoAceptada = 2;
        private const int IdEstacionCocina = 2;

        // Tasa de IVA (Costa Rica)
        private const decimal TasaImpuesto = 0.13m;

        private readonly IRepositoryPedido _repository;
        private readonly IServiceMenu _serviceMenu;
        private readonly IRepositoryProducto _repositoryProducto;
        private readonly IRepositoryCombo _repositoryCombo;

        public ServicePedido(
            IRepositoryPedido repository,
            IServiceMenu serviceMenu,
            IRepositoryProducto repositoryProducto,
            IRepositoryCombo repositoryCombo)
        {
            _repository = repository;
            _serviceMenu = serviceMenu;
            _repositoryProducto = repositoryProducto;
            _repositoryCombo = repositoryCombo;
        }

        public async Task<EstadisticasAdminDTO> ObtenerEstadisticasAdminAsync()
        {
            var cantidadPedidos = await _repository.ContarPedidosAsync();
            var masVendidos = await _repository.ObtenerProductosMasVendidosAsync(TopProductosMasVendidos);

            return new EstadisticasAdminDTO
            {
                CantidadPedidos = cantidadPedidos,
                ProductosMasVendidos = masVendidos
                    .Select(p => new ProductoVendidoDTO
                    {
                        Nombre = p.Nombre,
                        CantidadVendida = p.CantidadVendida
                    })
                    .ToList()
            };
        }

        // Catálogo de Caja: los productos y combos del menú disponible en este momento
        public async Task<ICollection<MenuItemDTO>> ObtenerCatalogoVentaAsync()
        {
            var menuDisponible = await _serviceMenu.GetMenuDisponibleAsync();
            if (menuDisponible == null)
                return new List<MenuItemDTO>();

            return menuDisponible.Items;
        }

        // Registra la venta: revalida cada ítem contra la base de datos (nunca confía en el
        // precio que mande el navegador), calcula impuesto/total y guarda todo en una transacción.
        public async Task<VentaResultadoDTO> CrearVentaAsync(VentaCrearDTO dto, int idEmpleado)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("El carrito está vacío.");

            var detalles = new List<DetallePedido>();

            foreach (var item in dto.Items)
            {
                if (item.Cantidad <= 0)
                    throw new InvalidOperationException("La cantidad de cada ítem debe ser mayor a cero.");

                if (item.Tipo == "Producto")
                {
                    var producto = await _repositoryProducto.FindByIdAsync(item.Id);
                    if (producto == null || !producto.Activo)
                        throw new InvalidOperationException($"El producto seleccionado ya no está disponible.");

                    detalles.Add(new DetallePedido
                    {
                        IdProducto = producto.IdProducto,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = producto.Precio * item.Cantidad,
                        Observaciones = item.Observaciones
                    });
                }
                else if (item.Tipo == "Combo")
                {
                    var combo = await _repositoryCombo.FindByIdAsync(item.Id);
                    if (combo == null || !combo.Activo)
                        throw new InvalidOperationException($"El combo seleccionado ya no está disponible.");

                    detalles.Add(new DetallePedido
                    {
                        IdCombo = combo.IdCombo,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = combo.PrecioEspecial,
                        Subtotal = combo.PrecioEspecial * item.Cantidad,
                        Observaciones = item.Observaciones
                    });
                }
                else
                {
                    throw new InvalidOperationException("Tipo de ítem inválido.");
                }
            }

            var subtotal = detalles.Sum(d => d.Subtotal);
            var impuesto = Math.Round(subtotal * TasaImpuesto, 2);
            var total = subtotal + impuesto;

            if (dto.MontoPagado < total)
                throw new InvalidOperationException("El monto pagado es menor al total de la venta.");

            var vuelto = dto.MontoPagado - total;

            var pedido = new Pedido
            {
                CodigoOrden = GenerarCodigoOrden(),
                FechaPedido = DateTime.Now,
                OrigenPedido = "Caja",
                MetodoEntrega = string.IsNullOrWhiteSpace(dto.MetodoEntrega) ? "Local" : dto.MetodoEntrega,
                Subtotal = subtotal,
                Impuesto = impuesto,
                CostoEnvio = 0,
                Total = total,
                IdEstado = IdEstadoAceptada,
                IdEstacionActual = IdEstacionCocina,
                IdEmpleado = idEmpleado,
                IdCliente = dto.IdCliente,
                NombreClienteInvitado = dto.IdCliente == null
                    ? (string.IsNullOrWhiteSpace(dto.NombreClienteInvitado) ? "Cliente Anónimo" : dto.NombreClienteInvitado)
                    : null,
                CedulaClienteInvitado = dto.IdCliente == null ? dto.CedulaClienteInvitado : null,
                DetallePedido = detalles,
                Pago = new List<Pago>
                {
                    new Pago
                    {
                        MontoPagado = dto.MontoPagado,
                        Vuelto = vuelto,
                        TipoTarjeta = dto.TipoTarjeta,
                        UltimosDigitos = dto.UltimosDigitos,
                        FechaPago = DateTime.Now,
                        IdMetodoPago = dto.IdMetodoPago
                    }
                }
            };

            await _repository.CrearAsync(pedido);

            return new VentaResultadoDTO
            {
                IdPedido = pedido.IdPedido,
                CodigoOrden = pedido.CodigoOrden,
                Subtotal = subtotal,
                Impuesto = impuesto,
                Total = total,
                MontoPagado = dto.MontoPagado,
                Vuelto = vuelto
            };
        }

        private static string GenerarCodigoOrden()
        {
            return $"ORD-{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
        }
    }
}
