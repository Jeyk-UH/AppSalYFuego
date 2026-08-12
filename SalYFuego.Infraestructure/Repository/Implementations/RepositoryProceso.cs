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
                    .ThenInclude(pp => pp.IdEstacionNavigation)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // Obtener producto con su proceso completo
        public async Task<Producto?> FindByIdAsync(int id)
        {
            return await _context.Set<Producto>()
                .Include(p => p.ProcesoPreparacion)
                    .ThenInclude(pp => pp.IdEstacionNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        // Agregar un paso al proceso
        public async Task AddProcesoAsync(ProcesoPreparacion proceso)
        {
            await _context.Set<ProcesoPreparacion>().AddAsync(proceso);
            await _context.SaveChangesAsync();
        }

        // Actualizar un paso del proceso
        public async Task UpdateProcesoAsync(ProcesoPreparacion proceso)
        {
            _context.Set<ProcesoPreparacion>().Update(proceso);
            await _context.SaveChangesAsync();
        }

        // Eliminar todos los pasos de un producto para reemplazarlos
        public async Task DeleteProcesosByProductoAsync(int idProducto)
        {
            var pasos = await _context.Set<ProcesoPreparacion>()
                .Where(p => p.IdProducto == idProducto)
                .ToListAsync();

            _context.Set<ProcesoPreparacion>().RemoveRange(pasos);
            await _context.SaveChangesAsync();
        }

        // Obtener todas las estaciones disponibles
        public async Task<ICollection<Estacion>> ListEstacionesAsync()
        {
            return await _context.Set<Estacion>()
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }
    }
}