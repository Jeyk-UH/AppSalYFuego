using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryProceso
    {
        // Lista de productos con sus procesos de preparación
        Task<ICollection<Producto>> ListAsync();
        // Lista de productos con sus procesos de preparación filtrados por categoría
        Task<Producto?> FindByIdAsync(int id);
        // Lista de productos con sus procesos de preparación filtrados por categoría
        Task AddProcesoAsync(ProcesoPreparacion proceso);
        // Actualiza un proceso de preparación existente
        Task UpdateProcesoAsync(ProcesoPreparacion proceso);
        // Elimina un proceso de preparación existente
        Task DeleteProcesosByProductoAsync(int idProducto);
        // Lista de estaciones de trabajo
        Task<ICollection<Estacion>> ListEstacionesAsync();
    }
}