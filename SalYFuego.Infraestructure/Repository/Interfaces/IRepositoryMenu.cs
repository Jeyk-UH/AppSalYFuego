
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryMenu
    {
        // Obtener lista de menus
        Task<ICollection<Menu>> ListAsync();
        // Obtener menu por id
        Task<Menu?> FindByIdAsync(int id);
        // Obtener menu disponible
        Task<Menu?> GetMenuDisponibleAsync();
        // Agregar menu
        Task AddAsync(Menu menu);
        // Actualizar menu
        Task UpdateAsync(Menu menu);
        // Desactivar menu
        Task DesactivarAsync(Menu menu);
    }
}