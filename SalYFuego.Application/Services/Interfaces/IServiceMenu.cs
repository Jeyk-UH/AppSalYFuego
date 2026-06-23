using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceMenu
    {
        Task<ICollection<MenuDTO>> ListAsync();
        Task<MenuDTO?> GetMenuDisponibleAsync();
    }
}