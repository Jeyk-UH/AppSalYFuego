using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly SalYFuegoContext _context;

        public RepositoryPedido(SalYFuegoContext context)
        {
            _context = context;
        }

        // Cantidad total de pedidos registrados en el sistema
        public async Task<int> ContarPedidosAsync()
        {
            return await _context.Set<Pedido>().CountAsync();
        }

        // Agrupa DetallePedido por producto y suma las cantidades vendidas
        public async Task<ICollection<ProductoVendidoResultado>> ObtenerProductosMasVendidosAsync(int top)
        {
            return await _context.Set<DetallePedido>()
                .Where(d => d.IdProducto != null)
                .GroupBy(d => new { d.IdProducto, Nombre = d.IdProductoNavigation!.Nombre })
                .Select(g => new ProductoVendidoResultado
                {
                    IdProducto = g.Key.IdProducto!.Value,
                    Nombre = g.Key.Nombre,
                    CantidadVendida = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(r => r.CantidadVendida)
                .Take(top)
                .ToListAsync();
        }

        // Inserta el Pedido junto con sus DetallePedido y Pago ya armados en las
        // colecciones de navegación; EF Core encadena las tres inserciones en un
        // solo SaveChanges y completa las FK automáticamente.
        public async Task<Pedido> CrearAsync(Pedido pedido)
        {
            await _context.Set<Pedido>().AddAsync(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        // Historial del Cliente logueado
        public async Task<ICollection<Pedido>> ListarPorClienteAsync(int idCliente)
        {
            return await _context.Set<Pedido>()
                .Include(p => p.IdEstadoNavigation)
                .Where(p => p.IdCliente == idCliente)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }

        // Historial completo (Encargado/Administrador), con filtro opcional por fecha y estado
        public async Task<ICollection<Pedido>> ListarTodosAsync(DateTime? fecha, int? idEstado)
        {
            var query = _context.Set<Pedido>()
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.IdEstadoNavigation)
                .AsQueryable();

            if (fecha.HasValue)
                query = query.Where(p => p.FechaPedido.Date == fecha.Value.Date);

            if (idEstado.HasValue)
                query = query.Where(p => p.IdEstado == idEstado.Value);

            return await query
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }

        // Pedido completo para armar el detalle en formato de factura
        public async Task<Pedido?> FindDetalleByIdAsync(int id)
        {
            return await _context.Set<Pedido>()
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.IdEmpleadoNavigation)
                .Include(p => p.IdEstadoNavigation)
                .Include(p => p.DetallePedido).ThenInclude(d => d.IdProductoNavigation)
                .Include(p => p.DetallePedido).ThenInclude(d => d.IdComboNavigation)
                .Include(p => p.Pago).ThenInclude(pa => pa.IdMetodoPagoNavigation)
                .FirstOrDefaultAsync(p => p.IdPedido == id);
        }
    }
}
