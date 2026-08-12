using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceProducto
    {
        Task<ICollection<ProductoDTO>> ListAsync();
        Task<ProductoDTO> FindByIdAsync(int id);
        //Mantenimiento
        Task<ProductoFormDTO?> FindFormByIdAsync(int id);
        Task AddAsync(ProductoFormDTO dto, string wwwrootPath);
        Task UpdateAsync(ProductoFormDTO dto, string wwwrootPath);
        Task DesactivarAsync(int id);
    }
}