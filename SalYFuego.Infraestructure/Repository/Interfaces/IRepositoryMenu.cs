
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryMenu
    {
        // Obtener todos los menús
        Task<ICollection<Menu>> ListAsync();
        // Obtener el menú disponible según día y hora actual
        Task<Menu?> GetMenuDisponibleAsync();
    }
}