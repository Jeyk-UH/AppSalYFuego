using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryMenu : IRepositoryMenu
    {
        private readonly SalYFuegoContext _context;

        public RepositoryMenu(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todos los menús con disponibilidad e items
        public async Task<ICollection<Menu>> ListAsync()
        {
            return await _context.Set<Menu>()
                .Include(m => m.MenuDisponibilidad)
                .Include(m => m.MenuItem)
                    .ThenInclude(mi => mi.IdProductoNavigation)
                .Include(m => m.MenuItem)
                    .ThenInclude(mi => mi.IdComboNavigation)
                .OrderByDescending(m => m.MenuDisponibilidad
                    .Max(d => d.FechaInicio ?? DateOnly.MinValue))
                .ToListAsync();
        }

        // Obtener menú por id con todas sus relaciones
        public async Task<Menu?> FindByIdAsync(int id)
        {
            return await _context.Set<Menu>()
                .Include(m => m.MenuDisponibilidad)
                .Include(m => m.MenuItem)
                    .ThenInclude(mi => mi.IdProductoNavigation)
                .Include(m => m.MenuItem)
                    .ThenInclude(mi => mi.IdComboNavigation)
                .FirstOrDefaultAsync(m => m.IdMenu == id);
        }

        // Obtener menú disponible según fecha y hora actual
        public async Task<Menu?> GetMenuDisponibleAsync()
        {
            var ahora = DateTime.Now;
            var horaActual = TimeOnly.FromDateTime(ahora);
            var fechaActual = DateOnly.FromDateTime(ahora);
            var diasSemana = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday,    "Lunes"     },
                { DayOfWeek.Tuesday,   "Martes"    },
                { DayOfWeek.Wednesday, "Miércoles" },
                { DayOfWeek.Thursday,  "Jueves"    },
                { DayOfWeek.Friday,    "Viernes"   },
                { DayOfWeek.Saturday,  "Sábado"    },
                { DayOfWeek.Sunday,    "Domingo"   }
            };
            var diaActual = diasSemana[ahora.DayOfWeek];

            return await _context.Set<Menu>()
                .Include(m => m.MenuDisponibilidad)
                .Include(m => m.MenuItem)
                    .ThenInclude(mi => mi.IdProductoNavigation)
                        .ThenInclude(p => p.IdCategoriaNavigation)
                .Include(m => m.MenuItem)
                    .ThenInclude(mi => mi.IdProductoNavigation)
                        .ThenInclude(p => p.ProductoImagen)
                .Include(m => m.MenuItem)
                    .ThenInclude(mi => mi.IdComboNavigation)
                        .ThenInclude(c => c.IdCategoriaNavigation)
                .Where(m => m.EstaActivo == true &&
                    m.HoraInicio <= horaActual &&
                    m.HoraFin >= horaActual &&
                    m.MenuDisponibilidad.Any(d =>
                        (d.DiaSemana == diaActual && d.FechaInicio == null) ||
                        (d.FechaInicio <= fechaActual && d.FechaFin >= fechaActual)
                    ))
                .FirstOrDefaultAsync();
        }

        // Agregar nuevo menú
        public async Task AddAsync(Menu menu)
        {
            await _context.Set<Menu>().AddAsync(menu);
            await _context.SaveChangesAsync();
        }

        // Actualizar menú existente
        // Actualizar menú existente
        public async Task UpdateAsync(Menu menu)
        {
            // Eliminar disponibilidades e items existentes antes de actualizar
            var disponibilidades = await _context.Set<MenuDisponibilidad>()
                .Where(d => d.IdMenu == menu.IdMenu)
                .ToListAsync();
            _context.Set<MenuDisponibilidad>().RemoveRange(disponibilidades);

            var items = await _context.Set<MenuItem>()
                .Where(i => i.IdMenu == menu.IdMenu)
                .ToListAsync();
            _context.Set<MenuItem>().RemoveRange(items);

            await _context.SaveChangesAsync();

            // Ahora actualizar el menú con los nuevos datos
            _context.Set<Menu>().Update(menu);
            await _context.SaveChangesAsync();
        }

        // Desactivar menú
        public async Task DesactivarAsync(Menu menu)
        {
            menu.EstaActivo = !menu.EstaActivo;
            _context.Set<Menu>().Update(menu);
            await _context.SaveChangesAsync();
        }
    }
}