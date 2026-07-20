using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryProducto
    {
        // Obtener todos los productos
        Task<ICollection<Producto>> ListAsync();
        // Obtener producto por id con ingredientes, categoria e imagen
        Task<Producto> FindByIdAsync(int id);
        //Mantenimientos
        Task AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(Producto producto);
    }
}