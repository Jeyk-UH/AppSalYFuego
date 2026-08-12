using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceProceso : IServiceProceso
    {
        private readonly IRepositoryProceso _repository;
        private readonly IMapper _mapper;

        public ServiceProceso(IRepositoryProceso repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<ProcesoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ProcesoDTO>>(list);
        }

        public async Task<ProcesoDTO?> FindByIdAsync(int id)
        {
            var producto = await _repository.FindByIdAsync(id);
            if (producto == null) return null;
            return _mapper.Map<ProcesoDTO>(producto);
        }

        // Cargar proceso para el formulario de edición
        public async Task<ProcesoFormDTO?> FindFormByIdAsync(int id)
        {
            var producto = await _repository.FindByIdAsync(id);
            if (producto == null) return null;

            return new ProcesoFormDTO
            {
                IdProducto = producto.IdProducto,
                NombreProducto = producto.Nombre,
                Pasos = producto.ProcesoPreparacion
                    .OrderBy(p => p.OrdenPaso)
                    .Select(p => new PasoFormDTO
                    {
                        OrdenPaso = p.OrdenPaso,
                        IdEstacion = p.IdEstacion,
                        NombreEstacion = p.IdEstacionNavigation?.Nombre,
                        TiempoEstimadoMinutos = p.TiempoEstimadoMinutos
                    }).ToList()
            };
        }

        // Guardar proceso (agregar o actualizar)
        public async Task SaveAsync(ProcesoFormDTO dto)
        {
            // Eliminar pasos existentes y reemplazar
            await _repository.DeleteProcesosByProductoAsync(dto.IdProducto);

            // Agregar nuevos pasos en orden
            for (int i = 0; i < dto.Pasos.Count; i++)
            {
                var paso = dto.Pasos[i];
                await _repository.AddProcesoAsync(new ProcesoPreparacion
                {
                    IdProducto = dto.IdProducto,
                    IdEstacion = paso.IdEstacion,
                    OrdenPaso = i + 1,
                    TiempoEstimadoMinutos = paso.TiempoEstimadoMinutos
                });
            }
        }

        public async Task<ICollection<EstacionDTO>> ListEstacionesAsync()
        {
            var list = await _repository.ListEstacionesAsync();
            return _mapper.Map<ICollection<EstacionDTO>>(list);
        }
    }
}