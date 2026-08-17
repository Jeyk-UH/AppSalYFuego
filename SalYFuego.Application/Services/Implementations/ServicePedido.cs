using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServicePedido : IServicePedido
    {
        private const int TopProductosMasVendidos = 5;

        // IdEstado (tabla ESTADO_PEDIDO, 10 estados reales — ver SalYFuegoDB_Maestro.sql):
        // 1 Pendiente de Pago, 2 Pagado, 3 En Preparación, 4 Preparado,
        // 5 En Espera Repartidor, 6 En Ruta, 7 Listo para Retirar, 8 Listo para Servir,
        // 9 Entregado, 10 Retirado.
        private const int IdEstadoPendientePago = 1;
        private const int IdEstadoPagado = 2;
        private const int IdEstadoEnPreparacion = 3;
        private const int IdEstadoPreparado = 4;
        private const int IdEstadoEnEsperaRepartidor = 5;
        private const int IdEstadoEnRuta = 6;
        private const int IdEstadoListoParaRetirar = 7;
        private const int IdEstadoListoParaServir = 8; // sin uso actual (no hay modo "consumo en el local")
        private const int IdEstadoEntregado = 9;
        private const int IdEstadoRetirado = 10;

        private const int IdEstacionCocina = 2;
        private const int IdEstacionCaja = 1;
        private const string MetodoEntregaDomicilio = "Entrega a domicilio";

        // Tramo común a todo pedido, antes de que se bifurque según el método de entrega.
        private static readonly int[] SecuenciaComun =
        {
            IdEstadoPendientePago, IdEstadoPagado, IdEstadoEnPreparacion, IdEstadoPreparado
        };

        // Después de "Preparado" el camino depende de si es retiro en tienda o domicilio.
        private static readonly int[] SecuenciaRecogida = { IdEstadoListoParaRetirar, IdEstadoRetirado };
        private static readonly int[] SecuenciaDomicilio = { IdEstadoEnEsperaRepartidor, IdEstadoEnRuta, IdEstadoEntregado };

        // Estados terminales del pedido (ya no se puede avanzar más), usados para
        // el filtro "solo activos" del historial.
        private static readonly int[] EstadosTerminales = { IdEstadoEntregado, IdEstadoRetirado };

        // Todos los IdEstado válidos (para validar cambios manuales de estado).
        private static readonly int[] TodosLosEstados = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Estados que Cocina debe atender: ya pagados y aún no despachados a la siguiente etapa.
        private static readonly int[] EstadosColaCocina = { IdEstadoPagado, IdEstadoEnPreparacion };

        // Secuencia completa de un pedido según su método de entrega (para AvanzarEstadoAsync).
        private static int[] ObtenerSecuenciaCompleta(string metodoEntrega)
        {
            var ramaFinal = metodoEntrega == MetodoEntregaDomicilio ? SecuenciaDomicilio : SecuenciaRecogida;
            return SecuenciaComun.Concat(ramaFinal).ToArray();
        }

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

            // Checkout asistido por Encargado con cliente registrado obligatorio:
            // si el cajero marcó "Cliente registrado", no se permite que la venta
            // termine sin un IdCliente real (evita degradar en silencio a anónima).
            if (dto.RequiereClienteRegistrado && dto.IdCliente == null)
                throw new InvalidOperationException("Debe seleccionar un cliente registrado para esta venta.");

            bool esDomicilio = dto.MetodoEntrega == MetodoEntregaDomicilio;

            // La entrega a domicilio necesita una dirección real asociada a un
            // usuario: no aplica para ventas anónimas (no hay a quién ligarla).
            if (esDomicilio && dto.IdCliente == null)
                throw new InvalidOperationException("La entrega a domicilio requiere un cliente registrado.");
            if (esDomicilio && string.IsNullOrWhiteSpace(dto.DireccionExacta))
                throw new InvalidOperationException("Debe indicar la dirección exacta de entrega.");

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
            var costoEnvio = esDomicilio ? TarifasEnvio.CostoDomicilio : 0m;
            var total = subtotal + impuesto + costoEnvio;

            if (dto.MontoPagado < total)
                throw new InvalidOperationException("El monto pagado es menor al total de la venta.");

            var vuelto = dto.MontoPagado - total;

            var pedido = new Pedido
            {
                CodigoOrden = GenerarCodigoOrden(),
                FechaPedido = DateTime.Now,
                OrigenPedido = "Caja",
                MetodoEntrega = string.IsNullOrWhiteSpace(dto.MetodoEntrega) ? "Recogida en tienda" : dto.MetodoEntrega,
                Subtotal = subtotal,
                Impuesto = impuesto,
                CostoEnvio = costoEnvio,
                Total = total,
                IdEstado = IdEstadoPagado,
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

            // Igual que en CrearPedidoClienteAsync: la dirección se inserta en
            // cascada junto con el Pedido (ya validamos arriba que hay IdCliente).
            if (esDomicilio)
            {
                pedido.IdDireccionEntregaNavigation = new DireccionUsuario
                {
                    IdUsuario = dto.IdCliente!.Value,
                    Alias = "Entrega a domicilio",
                    Provincia = dto.Provincia,
                    Canton = dto.Canton,
                    Distrito = dto.Distrito,
                    DireccionExacta = dto.DireccionExacta,
                    Referencia = dto.Referencia,
                    EsPredeterminada = false
                };
            }

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

        // Pedido registrado por el propio Cliente desde el carrito público. Igual que
        // CrearVentaAsync, revalida cada ítem contra la base de datos; a diferencia de
        // Caja, el cliente es obligatorio, no hay cobro presencial (queda Pendiente de
        // Pago) y puede llevar costo de envío si eligió entrega a domicilio.
        public async Task<VentaResultadoDTO> CrearPedidoClienteAsync(PedidoClienteCrearDTO dto, int idCliente)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("El carrito está vacío.");

            bool esDomicilio = dto.MetodoEntrega == MetodoEntregaDomicilio;
            if (esDomicilio && string.IsNullOrWhiteSpace(dto.DireccionExacta))
                throw new InvalidOperationException("Debe indicar la dirección exacta de entrega.");

            var detalles = new List<DetallePedido>();

            foreach (var item in dto.Items)
            {
                if (item.Cantidad <= 0)
                    throw new InvalidOperationException("La cantidad de cada ítem debe ser mayor a cero.");

                if (item.Tipo == "Producto")
                {
                    var producto = await _repositoryProducto.FindByIdAsync(item.Id);
                    if (producto == null || !producto.Activo)
                        throw new InvalidOperationException("Un producto del carrito ya no está disponible.");

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
                        throw new InvalidOperationException("Un combo del carrito ya no está disponible.");

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
            var costoEnvio = esDomicilio ? TarifasEnvio.CostoDomicilio : 0m;
            var total = subtotal + impuesto + costoEnvio;

            // Con tarjeta se simula el cobro inmediato en línea: el pedido nace
            // Pagado y con su registro de Pago ya creado. Con efectivo el cliente
            // paga en persona al retirar/recibir, así que el pedido queda
            // Pendiente de Pago y SIN registro de Pago hasta que el Encargado lo
            // cobre (ver ServicePedido.CobrarPedidoPendienteAsync).
            bool esTarjeta = !string.IsNullOrWhiteSpace(dto.TipoTarjeta);
            var montoPagado = esTarjeta ? total : 0m;
            var vuelto = 0m;

            var pedido = new Pedido
            {
                CodigoOrden = GenerarCodigoOrden(),
                FechaPedido = DateTime.Now,
                OrigenPedido = "Cliente",
                MetodoEntrega = dto.MetodoEntrega,
                Subtotal = subtotal,
                Impuesto = impuesto,
                CostoEnvio = costoEnvio,
                Total = total,
                IdEstado = esTarjeta ? IdEstadoPagado : IdEstadoPendientePago,
                IdEstacionActual = IdEstacionCaja,
                IdCliente = idCliente,
                DetallePedido = detalles,
                Pago = esTarjeta
                    ? new List<Pago>
                    {
                        new Pago
                        {
                            MontoPagado = montoPagado,
                            Vuelto = vuelto,
                            TipoTarjeta = dto.TipoTarjeta,
                            UltimosDigitos = dto.UltimosDigitos,
                            FechaPago = DateTime.Now,
                            IdMetodoPago = dto.IdMetodoPago
                        }
                    }
                    : new List<Pago>()
            };

            // Si es entrega a domicilio, la dirección se guarda como una nueva
            // DireccionUsuario del cliente; al no tener clave asignada, EF Core la
            // inserta en cascada junto con el Pedido en el mismo SaveChanges.
            if (esDomicilio)
            {
                pedido.IdDireccionEntregaNavigation = new DireccionUsuario
                {
                    IdUsuario = idCliente,
                    Alias = "Entrega a domicilio",
                    Provincia = dto.Provincia,
                    Canton = dto.Canton,
                    Distrito = dto.Distrito,
                    DireccionExacta = dto.DireccionExacta,
                    Referencia = dto.Referencia,
                    EsPredeterminada = false
                };
            }

            await _repository.CrearAsync(pedido);

            return new VentaResultadoDTO
            {
                IdPedido = pedido.IdPedido,
                CodigoOrden = pedido.CodigoOrden,
                Subtotal = subtotal,
                Impuesto = impuesto,
                Total = total,
                MontoPagado = montoPagado,
                Vuelto = vuelto
            };
        }

        // Historial del Cliente logueado
        public async Task<ICollection<PedidoListaDTO>> ObtenerHistorialClienteAsync(int idCliente)
        {
            var pedidos = await _repository.ListarPorClienteAsync(idCliente);
            return pedidos.Select(MapearALista).ToList();
        }

        // Historial completo (Encargado/Administrador), con filtro opcional por fecha, estado,
        // y "solo activos" (oculta los pedidos ya en un estado terminal: Entregado o Retirado)
        public async Task<ICollection<PedidoListaDTO>> ObtenerHistorialTodosAsync(DateTime? fecha, int? idEstado, bool soloActivos = false)
        {
            var idsEstadoExcluir = soloActivos ? EstadosTerminales : null;
            var pedidos = await _repository.ListarTodosAsync(fecha, idEstado, idsEstadoExcluir);
            return pedidos.Select(MapearALista).ToList();
        }

        private static PedidoListaDTO MapearALista(Pedido p) => new()
        {
            IdPedido = p.IdPedido,
            CodigoOrden = p.CodigoOrden,
            FechaPedido = p.FechaPedido,
            ClienteNombre = p.IdClienteNavigation?.NombreCompleto
                ?? p.NombreClienteInvitado
                ?? "Cliente Anónimo",
            EstadoNombre = p.IdEstadoNavigation.Nombre,
            Total = p.Total
        };

        // Detalle de un pedido en formato de factura, con el impuesto calculado por línea
        public async Task<PedidoDetalleDTO?> ObtenerDetalleAsync(int idPedido)
        {
            var p = await _repository.FindDetalleByIdAsync(idPedido);
            if (p == null) return null;

            return new PedidoDetalleDTO
            {
                IdPedido = p.IdPedido,
                IdCliente = p.IdCliente,
                CodigoOrden = p.CodigoOrden,
                FechaPedido = p.FechaPedido,
                ClienteNombre = p.IdClienteNavigation?.NombreCompleto
                    ?? p.NombreClienteInvitado
                    ?? "Cliente Anónimo",
                ClienteIdentificador = p.IdClienteNavigation?.Correo
                    ?? p.CedulaClienteInvitado
                    ?? "Sin cédula",
                EncargadoNombre = p.IdEmpleadoNavigation?.NombreCompleto ?? "—",
                MetodoEntrega = p.MetodoEntrega,
                MetodoPagoNombre = p.Pago.FirstOrDefault()?.IdMetodoPagoNavigation?.Nombre ?? "—",
                EstadoNombre = p.IdEstadoNavigation.Nombre,
                Lineas = p.DetallePedido.Select(d => new DetalleLineaDTO
                {
                    Nombre = d.IdProductoNavigation?.Nombre ?? d.IdComboNavigation?.Nombre ?? "—",
                    PrecioUnitario = d.PrecioUnitario,
                    Cantidad = d.Cantidad,
                    Subtotal = d.Subtotal,
                    Impuesto = Math.Round(d.Subtotal * TasaImpuesto, 2),
                    Observaciones = d.Observaciones
                }).ToList(),
                Subtotal = p.Subtotal,
                Impuesto = p.Impuesto,
                CostoEnvio = p.CostoEnvio,
                Total = p.Total
            };
        }

        // Cola de Cocina: pedidos Aceptados o en Preparación, del más antiguo al más
        // reciente (FIFO), sin precios (a Cocina no le hace falta esa información).
        public async Task<ICollection<PedidoColaDTO>> ObtenerColaCocinaAsync()
        {
            var pedidos = await _repository.ListarPorEstadosAsync(EstadosColaCocina);

            return pedidos.Select(p => new PedidoColaDTO
            {
                IdPedido = p.IdPedido,
                CodigoOrden = p.CodigoOrden,
                FechaPedido = p.FechaPedido,
                ClienteNombre = p.IdClienteNavigation?.NombreCompleto
                    ?? p.NombreClienteInvitado
                    ?? "Cliente Anónimo",
                IdEstado = p.IdEstado,
                EstadoNombre = p.IdEstadoNavigation.Nombre,
                Lineas = p.DetallePedido.Select(d => new DetalleLineaColaDTO
                {
                    Nombre = d.IdProductoNavigation?.Nombre ?? d.IdComboNavigation?.Nombre ?? "—",
                    Cantidad = d.Cantidad,
                    Observaciones = d.Observaciones
                }).ToList()
            }).ToList();
        }

        // Avanza el pedido al siguiente estado de SU secuencia (que depende del método
        // de entrega: recogida en tienda o domicilio). Usado por Cocina y, en general,
        // por cualquier avance automático paso a paso. Devuelve el nuevo IdEstado.
        public async Task<int> AvanzarEstadoAsync(int idPedido, int idUsuario)
        {
            var pedido = await _repository.FindParaActualizarEstadoAsync(idPedido);
            if (pedido == null)
                throw new InvalidOperationException("El pedido no existe.");

            var secuencia = ObtenerSecuenciaCompleta(pedido.MetodoEntrega);
            var indiceActual = Array.IndexOf(secuencia, pedido.IdEstado);
            if (indiceActual < 0 || indiceActual == secuencia.Length - 1)
                throw new InvalidOperationException("El pedido ya está en su estado final.");

            var nuevoEstado = secuencia[indiceActual + 1];
            await AplicarNuevoEstadoAsync(pedido, nuevoEstado, idUsuario);
            return nuevoEstado;
        }

        // Cambia el pedido a un estado específico, elegido manualmente por
        // Encargado/Administrador desde el Detalle de Pedido (corrección manual,
        // no sigue necesariamente la secuencia automática).
        public async Task CambiarEstadoAsync(int idPedido, int idEstadoNuevo, int idUsuario)
        {
            if (!TodosLosEstados.Contains(idEstadoNuevo))
                throw new InvalidOperationException("Estado inválido.");

            var pedido = await _repository.FindParaActualizarEstadoAsync(idPedido);
            if (pedido == null)
                throw new InvalidOperationException("El pedido no existe.");

            await AplicarNuevoEstadoAsync(pedido, idEstadoNuevo, idUsuario);
        }

        // Cobra un pedido que quedó Pendiente de Pago (por ejemplo, un pedido hecho
        // por el Cliente eligiendo pagar en efectivo al retirar/recibir). Crea el
        // registro de Pago con el vuelto correspondiente y avanza el pedido a Pagado.
        // Usado por Encargado/Administrador desde el botón "Cobrar" del Detalle.
        public async Task<VentaResultadoDTO> CobrarPedidoPendienteAsync(CobrarPedidoDTO dto, int idUsuario)
        {
            var pedido = await _repository.FindParaActualizarEstadoAsync(dto.IdPedido);
            if (pedido == null)
                throw new InvalidOperationException("El pedido no existe.");
            if (pedido.IdEstado != IdEstadoPendientePago)
                throw new InvalidOperationException("Este pedido ya fue cobrado.");

            if (dto.MontoPagado < pedido.Total)
                throw new InvalidOperationException("El monto recibido es menor al total del pedido.");
            var vuelto = dto.MontoPagado - pedido.Total;

            pedido.Pago.Add(new Pago
            {
                MontoPagado = dto.MontoPagado,
                Vuelto = vuelto,
                TipoTarjeta = dto.TipoTarjeta,
                UltimosDigitos = dto.UltimosDigitos,
                FechaPago = DateTime.Now,
                IdMetodoPago = dto.IdMetodoPago
            });

            await AplicarNuevoEstadoAsync(pedido, IdEstadoPagado, idUsuario);

            return new VentaResultadoDTO
            {
                IdPedido = pedido.IdPedido,
                CodigoOrden = pedido.CodigoOrden,
                Subtotal = pedido.Subtotal,
                Impuesto = pedido.Impuesto,
                Total = pedido.Total,
                MontoPagado = dto.MontoPagado,
                Vuelto = vuelto
            };
        }

        // Aplica el nuevo estado y deja registro en HISTORIAL_ESTADO_PEDIDO (quién y
        // cuándo lo cambió); se inserta en cascada junto con el Pedido ya trackeado.
        private async Task AplicarNuevoEstadoAsync(Pedido pedido, int idEstadoNuevo, int idUsuario)
        {
            pedido.IdEstado = idEstadoNuevo;
            pedido.HistorialEstadoPedido.Add(new HistorialEstadoPedido
            {
                IdEstado = idEstadoNuevo,
                FechaHora = DateTime.Now,
                IdUsuario = idUsuario
            });

            await _repository.GuardarCambiosEstadoAsync();
        }
    }
}
