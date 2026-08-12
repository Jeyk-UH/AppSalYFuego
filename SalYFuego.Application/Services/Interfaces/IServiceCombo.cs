using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceCombo
    {
        // Obtener lista de combos mapeados a DTO
        Task<ICollection<ComboDTO>> ListAsync();
        // Obtener lista de combos mapeados a DTO para el formulario
        Task<ComboDTO?> FindByIdAsync(int id);
        // Obtener lista de combos mapeados a DTO para el formulario
        Task<ComboFormDTO?> FindFormByIdAsync(int id);
        // Agregar un nuevo combo mapeado a DTO
        Task AddAsync(ComboFormDTO dto, string wwwrootPath);
        //  Actualizar un combo existente mapeado a DTO
        Task UpdateAsync(ComboFormDTO dto, string wwwrootPath);
        // Desactivar un combo existente mapeado a DTO
        Task DesactivarAsync(int id);
    }
}