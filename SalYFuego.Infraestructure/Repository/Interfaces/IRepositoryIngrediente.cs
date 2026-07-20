using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryIngrediente
    {
        Task<ICollection<Ingrediente>> ListAsync();
        Task<Ingrediente?> FindByIdAsync(int id);
        Task AddAsync(Ingrediente ingrediente);
        Task UpdateAsync(Ingrediente ingrediente);
        Task DeleteAsync(Ingrediente ingrediente);
    }
}