using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceProceso
    {
        Task<ICollection<ProcesoDTO>> ListAsync();
        Task<ProcesoDTO?> FindByIdAsync(int id);
    }
}