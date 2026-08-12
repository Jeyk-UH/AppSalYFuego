using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryUsuario
    {
        Task<ICollection<Usuario>> ListAsync();
        Task<Usuario?> FindByIdAsync(int id);
        Task<Usuario?> FindByCorreoAsync(string correo);
        // Busca el usuario por correo y contraseña ya encriptada, solo si está activo
        Task<Usuario?> LoginAsync(string correo, string passwordEncriptado);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        // Valida si ya existe un correo registrado (excluyendo opcionalmente un usuario, útil al editar)
        Task<bool> ExisteCorreoAsync(string correo, int? idUsuarioExcluir = null);
    }
}
