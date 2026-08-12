using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceMenu
    {
        // Obtener lista de menus
        Task<ICollection<MenuDTO>> ListAsync();
        // Obtener menu disponible
        Task<MenuDTO?> GetMenuDisponibleAsync();
        // Obtener menu por id
        Task<MenuFormDTO?> FindFormByIdAsync(int id);
        // Agregar menu
        Task AddAsync(MenuFormDTO dto);
        // Actualizar menu
        Task UpdateAsync(MenuFormDTO dto);
        // Desactivar menu
        Task DesactivarAsync(int id);
    }
}