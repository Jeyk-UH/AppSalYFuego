using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryProducto : IRepositoryProducto
    {
        private readonly SalYFuegoContext _context;

        public RepositoryProducto(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todos los productos con su imagen principal y categoria
        public async Task<ICollection<Producto>> ListAsync()
        {
            return await _context.Set<Producto>()
                .Where(p => p.Activo == true)
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoImagen)
                .ToListAsync();
        }

        // Obtener producto por id incluyendo ingredientes, categoria e imagenes
        public async Task<Producto> FindByIdAsync(int id)
        {
            return await _context.Set<Producto>()
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdIngrediente)
                .Include(p => p.ProductoImagen)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }
    }
}