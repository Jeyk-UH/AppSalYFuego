using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryCombo
    {
        // Obtener lista de combos
        Task<ICollection<Combo>> ListAsync();
        //  Obtener combo por id
        Task<Combo?> FindByIdAsync(int id);
        //  Agregar combo
        Task AddAsync(Combo combo);
        //  Actualizar combo
        Task UpdateAsync(Combo combo);
        //  Desactivar combo
        Task DesactivarAsync(Combo combo);
    }
}