using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceCombo
    {
        Task<ICollection<ComboDTO>> ListAsync();
        Task<ComboDTO> FindByIdAsync(int id);
    }
}