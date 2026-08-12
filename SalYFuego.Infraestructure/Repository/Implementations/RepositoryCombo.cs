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

        // Obtener todos los combos con categoria y productos
        public async Task<ICollection<Combo>> ListAsync()
        {
            return await _context.Set<Combo>()
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.ComboProducto)
                    .ThenInclude(cp => cp.IdProductoNavigation)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        // Obtener combo por id con todas sus relaciones
        public async Task<Combo?> FindByIdAsync(int id)
        {
            return await _context.Set<Combo>()
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.ComboProducto)
                    .ThenInclude(cp => cp.IdProductoNavigation)
                .FirstOrDefaultAsync(c => c.IdCombo == id);
        }

        // Agregar nuevo combo
        public async Task AddAsync(Combo combo)
        {
            await _context.Set<Combo>().AddAsync(combo);
            await _context.SaveChangesAsync();
        }

        // Actualizar combo existente
        public async Task UpdateAsync(Combo combo)
        {
            _context.Set<Combo>().Update(combo);
            await _context.SaveChangesAsync();
        }

        // Desactivar combo en vez de eliminarlo
        public async Task DesactivarAsync(Combo combo)
        {
            combo.Activo = !combo.Activo;
            _context.Set<Combo>().Update(combo);
            await _context.SaveChangesAsync();
        }
    }
}