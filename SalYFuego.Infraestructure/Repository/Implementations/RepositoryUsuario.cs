using Microsoft.EntityFrameworkCore;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Models;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Infraestructure.Repository.Implementations
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly SalYFuegoContext _context;

        public RepositoryUsuario(SalYFuegoContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios con su rol
        public async Task<ICollection<Usuario>> ListAsync()
        {
            return await _context.Set<Usuario>()
                .Include(u => u.IdRolNavigation)
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();
        }

        // Obtener usuario por id
        public async Task<Usuario?> FindByIdAsync(int id)
        {
            return await _context.Set<Usuario>()
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        // Obtener usuario por correo
        public async Task<Usuario?> FindByCorreoAsync(string correo)
        {
            return await _context.Set<Usuario>()
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Correo == correo);
        }

        // Verificar acceso: correo + contraseña ya encriptada, usuario activo
        public async Task<Usuario?> LoginAsync(string correo, string passwordEncriptado)
        {
            return await _context.Set<Usuario>()
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Correo == correo
                    && u.ContrasenaHash == passwordEncriptado
                    && u.Activo);
        }

        // Agregar nuevo usuario
        public async Task AddAsync(Usuario usuario)
        {
            await _context.Set<Usuario>().AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        // Actualizar usuario existente
        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Set<Usuario>().Update(usuario);
            await _context.SaveChangesAsync();
        }

        // Validar correo único (excluyendo el propio usuario al editar)
        public async Task<bool> ExisteCorreoAsync(string correo, int? idUsuarioExcluir = null)
        {
            return await _context.Set<Usuario>()
                .AnyAsync(u => u.Correo == correo
                    && (idUsuarioExcluir == null || u.IdUsuario != idUsuarioExcluir));
        }

        // Busca usuarios activos de un rol específico por nombre, correo o cédula
        public async Task<ICollection<Usuario>> BuscarPorRolAsync(int idRol, string termino)
        {
            return await _context.Set<Usuario>()
                .Where(u => u.IdRol == idRol
                    && u.Activo
                    && (u.NombreCompleto.Contains(termino)
                        || u.Correo.Contains(termino)
                        || (u.Cedula != null && u.Cedula.Contains(termino))))
                .OrderBy(u => u.NombreCompleto)
                .Take(10)
                .ToListAsync();
        }
    }
}
