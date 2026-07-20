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

        // Obtener todos los productos con categoria, imagen e ingredientes
        public async Task<ICollection<Producto>> ListAsync()
        {
            return await _context.Set<Producto>()
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoImagen)
                .Include(p => p.IdIngrediente)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // Obtener producto por id con todas sus relaciones
        public async Task<Producto?> FindByIdAsync(int id)
        {
            return await _context.Set<Producto>()
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.ProductoImagen)
                .Include(p => p.IdIngrediente)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        // Agregar nuevo producto
        public async Task AddAsync(Producto producto)
        {
            await _context.Set<Producto>().AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        // Actualizar producto existente
        public async Task UpdateAsync(Producto producto)
        {
            _context.Set<Producto>().Update(producto);
            await _context.SaveChangesAsync();
        }

        // Eliminar producto
        public async Task DeleteAsync(Producto producto)
        {
            _context.Set<Producto>().Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}