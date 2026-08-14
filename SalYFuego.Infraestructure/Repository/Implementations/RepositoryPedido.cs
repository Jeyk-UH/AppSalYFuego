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
    }
}
