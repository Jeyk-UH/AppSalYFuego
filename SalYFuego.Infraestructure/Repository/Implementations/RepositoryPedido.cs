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

        // Historial completo (Encargado/Administrador), con filtro opcional por fecha, estado,
        // y exclusión de un estado (usado para "ocultar entregados")
        public async Task<ICollection<Pedido>> ListarTodosAsync(DateTime? fecha, int? idEstado, int[]? idsEstadoExcluir = null)
        {
            var query = _context.Set<Pedido>()
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.IdEstadoNavigation)
                .AsQueryable();

            if (fecha.HasValue)
                query = query.Where(p => p.FechaPedido.Date == fecha.Value.Date);

            if (idEstado.HasValue)
                query = query.Where(p => p.IdEstado == idEstado.Value);

            if (idsEstadoExcluir is { Length: > 0 })
                query = query.Where(p => !idsEstadoExcluir.Contains(p.IdEstado));

            return await query
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }

        // Cola de Cocina: pedidos en alguno de los estados indicados, del más antiguo al
        // más reciente (FIFO), con las líneas de detalle ya cargadas para armar la comanda.
        public async Task<ICollection<Pedido>> ListarPorEstadosAsync(int[] idsEstado)
        {
            return await _context.Set<Pedido>()
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.IdEstadoNavigation)
                .Include(p => p.IdDireccionEntregaNavigation)
                .Include(p => p.DetallePedido).ThenInclude(d => d.IdProductoNavigation)
                .Include(p => p.DetallePedido).ThenInclude(d => d.IdComboNavigation)
                .Where(p => idsEstado.Contains(p.IdEstado))
                .OrderBy(p => p.FechaPedido)
                .ToListAsync();
        }

        // Los "top" pedidos más recientes entre los estados indicados (fecha descendente).
        // Usado para la columna "Finalizado" del tablero: solo los últimos N.
        public async Task<ICollection<Pedido>> ListarUltimosPorEstadosAsync(int[] idsEstado, int top)
        {
            return await _context.Set<Pedido>()
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.IdEstadoNavigation)
                .Where(p => idsEstado.Contains(p.IdEstado))
                .OrderByDescending(p => p.FechaPedido)
                .Take(top)
                .ToListAsync();
        }

        // Pedido liviano (sin includes) trackeado por EF, para actualizar solo su estado
        public async Task<Pedido?> FindParaActualizarEstadoAsync(int id)
        {
            return await _context.Set<Pedido>().FirstOrDefaultAsync(p => p.IdPedido == id);
        }

        public async Task GuardarCambiosEstadoAsync()
        {
            await _context.SaveChangesAsync();
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
