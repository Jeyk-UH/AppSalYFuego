using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryProceso : IRepositoryProceso
    {
        private readonly SalYFuegoContext _context;

        public RepositoryProceso(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todos los productos que tienen proceso de preparación
        public async Task<ICollection<Producto>> ListAsync()
        {
            return await _context.Set<Producto>()
                .Where(p => p.ProcesoPreparacion.Any())
                .Include(p => p.ProcesoPreparacion)
                .ToListAsync();
        }

        // Obtener producto con su proceso de preparación completo
        public async Task<Producto> FindByIdAsync(int id)
        {
            return await _context.Set<Producto>()
                .Include(p => p.ProcesoPreparacion)
                    .ThenInclude(pp => pp.IdEstacionNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }
    }
}