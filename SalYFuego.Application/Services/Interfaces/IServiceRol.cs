using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceRol
    {
        Task<ICollection<RolDTO>> ListAsync();
    }
}
