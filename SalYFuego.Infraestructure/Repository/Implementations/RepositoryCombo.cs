using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryCombo : IRepositoryCombo
    {
        private readonly SalYFuegoContext _context;

        public RepositoryCombo(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todos los combos activos con categoria e imagen
        public async Task<ICollection<Combo>> ListAsync()
        {
            return await _context.Set<Combo>()
                .Where(c => c.Activo == true)
                .Include(c => c.IdCategoriaNavigation)
                .ToListAsync();
        }

        // Obtener combo por id con productos que lo componen
        public async Task<Combo> FindByIdAsync(int id)
        {
            return await _context.Set<Combo>()
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.ComboProducto)
                    .ThenInclude(cp => cp.IdProductoNavigation)
                .FirstOrDefaultAsync(c => c.IdCombo == id);
        }
    }
}