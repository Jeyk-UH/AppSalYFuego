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

        // Obtener todos los menús ordenados por disponibilidad más reciente
        public async Task<ICollection<Menu>> ListAsync()
        {
            return await _context.Set<Menu>()
                .Include(m => m.MenuDisponibilidad)
                .OrderByDescending(m => m.MenuDisponibilidad
                    .Max(d => d.FechaInicio ?? DateOnly.MinValue))
                .ToListAsync();
        }

        // Obtener el menú disponible según fecha y hora actual
        public async Task<Menu> GetMenuDisponibleAsync()
        {
            var ahora = DateTime.Now;
            var horaActual = TimeOnly.FromDateTime(ahora);
            var fechaActual = DateOnly.FromDateTime(ahora);
            // Nombre del día actual en español
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
                        // Por día de semana
                        (d.DiaSemana == diaActual && d.FechaInicio == null) ||
                        // Por rango de fechas
                        (d.FechaInicio <= fechaActual && d.FechaFin >= fechaActual)
                    ))
                .FirstOrDefaultAsync();
        }
    }
}