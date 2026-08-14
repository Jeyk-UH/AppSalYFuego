using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryMetodoPago : IRepositoryMetodoPago
    {
        private readonly SalYFuegoContext _context;

        public RepositoryMetodoPago(SalYFuegoContext context)
        {
            _context = context;
        }

        public async Task<ICollection<MetodoPago>> ListAsync()
        {
            return await _context.Set<MetodoPago>()
                .OrderBy(m => m.IdMetodoPago)
                .ToListAsync();
        }
    }
}
