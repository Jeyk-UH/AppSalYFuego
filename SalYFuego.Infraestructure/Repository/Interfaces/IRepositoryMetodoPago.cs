using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryMetodoPago
    {
        Task<ICollection<MetodoPago>> ListAsync();
    }
}
