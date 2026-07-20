using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryCategoria : IRepositoryCategoria
    {
        private readonly SalYFuegoContext _context;

        public RepositoryCategoria(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todas las categorías ordenadas por nombre
        public async Task<ICollection<Categoria>> ListAsync()
        {
            return await _context.Set<Categoria>()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }
    }
}