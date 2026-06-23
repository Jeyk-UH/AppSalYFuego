using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryProceso
    {
        // Obtener todos los procesos agrupados por producto
        Task<ICollection<Producto>> ListAsync();
        // Obtener proceso de preparación de un producto por id
        Task<Producto> FindByIdAsync(int id);
    }
}