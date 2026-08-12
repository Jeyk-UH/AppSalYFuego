using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceProceso
    {
        //Listar todos los procesos
        Task<ICollection<ProcesoDTO>> ListAsync();
        //Buscar un proceso por su id
        Task<ProcesoDTO?> FindByIdAsync(int id);
        //Listar todos los formularios de un proceso
        Task<ProcesoFormDTO?> FindFormByIdAsync(int id);
        //Guardar un proceso
        Task SaveAsync(ProcesoFormDTO dto);
        //actualizar un proceso
        Task<ICollection<EstacionDTO>> ListEstacionesAsync();
    }
}