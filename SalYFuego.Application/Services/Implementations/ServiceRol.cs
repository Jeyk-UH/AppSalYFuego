using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceRol : IServiceRol
    {
        private readonly IRepositoryRol _repository;
        private readonly IMapper _mapper;

        public ServiceRol(IRepositoryRol repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Lista todos los roles (para el select del formulario de usuarios)
        public async Task<ICollection<RolDTO>> ListAsync()
        {
            var roles = await _repository.ListAsync();
            return _mapper.Map<ICollection<RolDTO>>(roles);
        }
    }
}
