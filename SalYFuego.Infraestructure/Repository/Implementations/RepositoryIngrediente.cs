using Microsoft.EntityFrameworkCore;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryIngrediente : IRepositoryIngrediente
    {
        private readonly SalYFuegoContext _context;

        public RepositoryIngrediente(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todos los ingredientes
        public async Task<ICollection<Ingrediente>> ListAsync()
        {
            return await _context.Set<Ingrediente>()
                .OrderBy(i => i.Nombre)
                .ToListAsync();
        }

        // Obtener ingrediente por id
        public async Task<Ingrediente?> FindByIdAsync(int id)
        {
            return await _context.Set<Ingrediente>()
                .FirstOrDefaultAsync(i => i.IdIngrediente == id);
        }

        // Agregar nuevo ingrediente
        public async Task AddAsync(Ingrediente ingrediente)
        {
            await _context.Set<Ingrediente>().AddAsync(ingrediente);
            await _context.SaveChangesAsync();
        }

        // Actualizar ingrediente existente
        public async Task UpdateAsync(Ingrediente ingrediente)
        {
            _context.Set<Ingrediente>().Update(ingrediente);
            await _context.SaveChangesAsync();
        }

        // Eliminar ingrediente
        public async Task DeleteAsync(Ingrediente ingrediente)
        {
            _context.Set<Ingrediente>().Remove(ingrediente);
            await _context.SaveChangesAsync();
        }
    }
}