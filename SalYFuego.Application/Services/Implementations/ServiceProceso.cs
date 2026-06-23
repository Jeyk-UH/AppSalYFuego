using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

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

        // Obtener lista de procesos mapeados a DTO
        public async Task<ICollection<ProcesoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ProcesoDTO>>(list);
        }

        // Obtener detalle de proceso por id de producto
        public async Task<ProcesoDTO?> FindByIdAsync(int id)
        {
            var producto = await _repository.FindByIdAsync(id);
            if (producto == null) return null;
            return _mapper.Map<ProcesoDTO>(producto);
        }
    }
}