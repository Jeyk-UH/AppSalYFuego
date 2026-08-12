using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceMenu : IServiceMenu
    {
        private readonly IRepositoryMenu _repository;
        private readonly IMapper _mapper;

        public ServiceMenu(IRepositoryMenu repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MenuDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuDTO>>(list);
        }

        public async Task<MenuDTO?> GetMenuDisponibleAsync()
        {
            var menu = await _repository.GetMenuDisponibleAsync();
            if (menu == null) return null;
            return _mapper.Map<MenuDTO>(menu);
        }

        // Cargar datos del menú para el formulario de edición
        public async Task<MenuFormDTO?> FindFormByIdAsync(int id)
        {
            var menu = await _repository.FindByIdAsync(id);
            if (menu == null) return null;

            // Determinar tipo de disponibilidad
            var primeraDisp = menu.MenuDisponibilidad.FirstOrDefault();
            var tipo = primeraDisp?.DiaSemana != null ? "dias" : "fechas";

            return new MenuFormDTO
            {
                IdMenu = menu.IdMenu,
                Nombre = menu.Nombre,
                HoraInicio = menu.HoraInicio,
                HoraFin = menu.HoraFin,
                EstaActivo = menu.EstaActivo,
                TipoDisponibilidad = tipo,
                DiasSeleccionados = menu.MenuDisponibilidad
                    .Where(d => d.DiaSemana != null)
                    .Select(d => d.DiaSemana!)
                    .ToList(),
                FechaInicio = primeraDisp?.FechaInicio,
                FechaFin = primeraDisp?.FechaFin,
                ProductosSeleccionados = menu.MenuItem
                    .Where(mi => mi.IdProducto != null)
                    .Select(mi => mi.IdProducto!.Value)
                    .ToList(),
                CombosSeleccionados = menu.MenuItem
                    .Where(mi => mi.IdCombo != null)
                    .Select(mi => mi.IdCombo!.Value)
                    .ToList()
            };
        }

        // Agregar nuevo menú
        public async Task AddAsync(MenuFormDTO dto)
        {
            var menu = new Menu
            {
                Nombre = dto.Nombre,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                EstaActivo = dto.EstaActivo
            };

            // Agregar disponibilidad
            AgregarDisponibilidad(menu, dto);

            // Agregar productos al menú
            foreach (var idProducto in dto.ProductosSeleccionados)
                menu.MenuItem.Add(new MenuItem { IdProducto = idProducto });

            // Agregar combos al menú
            foreach (var idCombo in dto.CombosSeleccionados)
                menu.MenuItem.Add(new MenuItem { IdCombo = idCombo });

            await _repository.AddAsync(menu);
        }

        // Actualizar menú existente
        public async Task UpdateAsync(MenuFormDTO dto)
        {
            var menu = await _repository.FindByIdAsync(dto.IdMenu);
            if (menu == null) return;

            menu.Nombre = dto.Nombre;
            menu.HoraInicio = dto.HoraInicio;
            menu.HoraFin = dto.HoraFin;
            menu.EstaActivo = dto.EstaActivo;

            // Limpiar y actualizar disponibilidad
            menu.MenuDisponibilidad.Clear();
            AgregarDisponibilidad(menu, dto);

            // Limpiar y actualizar items
            menu.MenuItem.Clear();
            foreach (var idProducto in dto.ProductosSeleccionados)
                menu.MenuItem.Add(new MenuItem
                {
                    IdMenu = menu.IdMenu,
                    IdProducto = idProducto
                });

            foreach (var idCombo in dto.CombosSeleccionados)
                menu.MenuItem.Add(new MenuItem
                {
                    IdMenu = menu.IdMenu,
                    IdCombo = idCombo
                });

            await _repository.UpdateAsync(menu);
        }

        // Desactivar menú
        public async Task DesactivarAsync(int id)
        {
            var menu = await _repository.FindByIdAsync(id);
            if (menu != null)
                await _repository.DesactivarAsync(menu);
        }

        // Método auxiliar para agregar disponibilidad
        private void AgregarDisponibilidad(Menu menu, MenuFormDTO dto)
        {
            if (dto.TipoDisponibilidad == "dias")
            {
                foreach (var dia in dto.DiasSeleccionados)
                    menu.MenuDisponibilidad.Add(new MenuDisponibilidad
                    {
                        DiaSemana = dia,
                        FechaInicio = null,
                        FechaFin = null
                    });
            }
            else
            {
                menu.MenuDisponibilidad.Add(new MenuDisponibilidad
                {
                    DiaSemana = null,
                    FechaInicio = dto.FechaInicio,
                    FechaFin = dto.FechaFin
                });
            }
        }
    }
}