using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceIngrediente
    {
        Task<ICollection<IngredienteDTO>> ListAsync();
        Task<IngredienteDTO?> FindByIdAsync(int id);
        Task AddAsync(IngredienteDTO dto);
        Task UpdateAsync(IngredienteDTO dto);
        Task DeleteAsync(int id);
    }
}