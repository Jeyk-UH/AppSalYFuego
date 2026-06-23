using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryCombo
    {
        // Obtener todos los combos
        Task<ICollection<Combo>> ListAsync();
        // Obtener combo por id con sus productos
        Task<Combo> FindByIdAsync(int id);
    }
}