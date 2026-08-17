using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceEstadoPedido
    {
        Task<ICollection<EstadoPedidoDTO>> ListAsync();
    }
}
