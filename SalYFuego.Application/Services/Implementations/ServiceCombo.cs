using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceCombo : IServiceCombo
    {
        private readonly IRepositoryCombo _repository;
        private readonly IMapper _mapper;

        public ServiceCombo(IRepositoryCombo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Obtener lista de combos mapeados a DTO
        public async Task<ICollection<ComboDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ComboDTO>>(list);
        }

        // Obtener detalle de combo por id mapeado a DTO
        public async Task<ComboDTO> FindByIdAsync(int id)
        {
            var combo = await _repository.FindByIdAsync(id);
            return _mapper.Map<ComboDTO>(combo);
        }
    }
}