using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceUsuario
    {
        Task<ICollection<UsuarioDTO>> ListAsync();
        Task<UsuarioFormDTO?> FindFormByIdAsync(int id);

        // Verifica credenciales (comparando contraseñas encriptadas). Retorna null si son inválidas.
        Task<UsuarioDTO?> LoginAsync(string correo, string password);

        // Retorna un mensaje de error si algo falla, o null si todo salió bien
        Task<string?> AddAsync(UsuarioFormDTO dto);
        Task<string?> UpdateAsync(UsuarioFormDTO dto);
        Task ToggleActivoAsync(int id);

        // Autoregistro público de clientes
        Task<string?> RegistrarClienteAsync(RegistroDTO dto);
    }
}
