using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryRol
    {
        Task<ICollection<Rol>> ListAsync();
        Task<Rol?> FindByIdAsync(int id);
    }
}
