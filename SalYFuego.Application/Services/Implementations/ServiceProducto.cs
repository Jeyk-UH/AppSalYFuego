using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceProducto : IServiceProducto
    {
        private readonly IRepositoryProducto _repository;
        private readonly IMapper _mapper;

        public ServiceProducto(IRepositoryProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Obtener lista de productos mapeados a DTO
        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ProductoDTO>>(list);
        }

        // Obtener detalle de producto por id mapeado a DTO
        public async Task<ProductoDTO> FindByIdAsync(int id)
        {
            var producto = await _repository.FindByIdAsync(id);
            return _mapper.Map<ProductoDTO>(producto);
        }
    }
}