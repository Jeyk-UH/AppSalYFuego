using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryEstadoPedido : IRepositoryEstadoPedido
    {
        private readonly SalYFuegoContext _context;

        public RepositoryEstadoPedido(SalYFuegoContext context)
        {
            _context = context;
        }

        public async Task<ICollection<EstadoPedido>> ListAsync()
        {
            return await _context.Set<EstadoPedido>()
                .OrderBy(e => e.Orden)
                .ToListAsync();
        }
    }
}
