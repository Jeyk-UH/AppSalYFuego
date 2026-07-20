using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using SalYFuego.Infraestructure.Models;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceIngrediente : IServiceIngrediente
    {
        private readonly IRepositoryIngrediente _repository;
        private readonly IMapper _mapper;

        public ServiceIngrediente(IRepositoryIngrediente repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<IngredienteDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<IngredienteDTO>>(list);
        }

        public async Task<IngredienteDTO?> FindByIdAsync(int id)
        {
            var ingrediente = await _repository.FindByIdAsync(id);
            if (ingrediente == null) return null;
            return _mapper.Map<IngredienteDTO>(ingrediente);
        }

        // Agregar nuevo ingrediente
        public async Task AddAsync(IngredienteDTO dto)
        {
            var ingrediente = _mapper.Map<Ingrediente>(dto);
            await _repository.AddAsync(ingrediente);
        }

        // Actualizar ingrediente existente
        public async Task UpdateAsync(IngredienteDTO dto)
        {
            var ingrediente = _mapper.Map<Ingrediente>(dto);
            await _repository.UpdateAsync(ingrediente);
        }

        // Eliminar ingrediente por id
        public async Task DeleteAsync(int id)
        {
            var ingrediente = await _repository.FindByIdAsync(id);
            if (ingrediente != null)
                await _repository.DeleteAsync(ingrediente);
        }
    }
}