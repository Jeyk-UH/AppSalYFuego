using Microsoft.EntityFrameworkCore;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryRol : IRepositoryRol
    {
        private readonly SalYFuegoContext _context;

        public RepositoryRol(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todos los roles
        public async Task<ICollection<Rol>> ListAsync()
        {
            return await _context.Set<Rol>()
                .OrderBy(r => r.IdRol)
                .ToListAsync();
        }

        // Obtener rol por id
        public async Task<Rol?> FindByIdAsync(int id)
        {
            return await _context.Set<Rol>()
                .FirstOrDefaultAsync(r => r.IdRol == id);
        }
    }
}
