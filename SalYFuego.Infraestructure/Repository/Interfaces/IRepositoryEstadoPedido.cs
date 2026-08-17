using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryEstadoPedido
    {
        Task<ICollection<EstadoPedido>> ListAsync();
    }
}
