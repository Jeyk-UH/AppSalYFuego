using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

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

        // Obtener lista de menús mapeados a DTO
        public async Task<ICollection<MenuDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuDTO>>(list);
        }

        // Obtener menú disponible mapeado a DTO
        public async Task<MenuDTO?> GetMenuDisponibleAsync()
        {
            var menu = await _repository.GetMenuDisponibleAsync();
            if (menu == null) return null;
            return _mapper.Map<MenuDTO>(menu);
        }
    }
}