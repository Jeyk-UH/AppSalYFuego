using AutoMapper;
using Microsoft.Extensions.Options;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Aplication.Utils;
using SalYFuego.Infraestructure.Models;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        // IdRol del rol Cliente (tabla ROL): 1-Administrador, 2-Encargado,
        // 3-Cocina, 4-Cliente, 5-Salonero, 6-Repartidor
        private const int IdRolCliente = 4;

        private readonly IRepositoryUsuario _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<AppConfig> _options;

        public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper, IOptions<AppConfig> options)
        {
            _repository = repository;
            _mapper = mapper;
            _options = options;
        }

        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {
            var usuarios = await _repository.ListAsync();
            return _mapper.Map<ICollection<UsuarioDTO>>(usuarios);
        }

        public async Task<UsuarioFormDTO?> FindFormByIdAsync(int id)
        {
            var usuario = await _repository.FindByIdAsync(id);
            if (usuario == null) return null;
            return _mapper.Map<UsuarioFormDTO>(usuario);
        }

        // Verificar sí el usuario existe y la contraseña coincide (comparando encriptado, no desencriptando)
        public async Task<UsuarioDTO?> LoginAsync(string correo, string password)
        {
            string secret = _options.Value.Crypto.Secret;
            string passwordEncriptado = Cryptography.Encrypt(password, secret);

            var usuario = await _repository.LoginAsync(correo, passwordEncriptado);
            if (usuario == null) return null;

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        // Agregar nuevo usuario (lo crea el Administrador)
        public async Task<string?> AddAsync(UsuarioFormDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                return "La contraseña es obligatoria.";

            if (await _repository.ExisteCorreoAsync(dto.Correo!))
                return "Ya existe un usuario registrado con ese correo.";

            string secret = _options.Value.Crypto.Secret;

            var usuario = new Usuario
            {
                NombreCompleto = dto.NombreCompleto!,
                Correo = dto.Correo!,
                ContrasenaHash = Cryptography.Encrypt(dto.Password, secret),
                IdRol = dto.IdRol,
                Activo = dto.Activo,
                Cedula = dto.Cedula
            };

            await _repository.AddAsync(usuario);
            return null;
        }

        // Actualizar usuario existente. La contraseña solo se cambia si se envía una nueva.
        public async Task<string?> UpdateAsync(UsuarioFormDTO dto)
        {
            var usuario = await _repository.FindByIdAsync(dto.IdUsuario);
            if (usuario == null)
                return "El usuario no existe.";

            if (await _repository.ExisteCorreoAsync(dto.Correo!, dto.IdUsuario))
                return "Ya existe otro usuario registrado con ese correo.";

            usuario.NombreCompleto = dto.NombreCompleto!;
            usuario.Correo = dto.Correo!;
            usuario.IdRol = dto.IdRol;
            usuario.Activo = dto.Activo;
            usuario.Cedula = dto.Cedula;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                string secret = _options.Value.Crypto.Secret;
                usuario.ContrasenaHash = Cryptography.Encrypt(dto.Password, secret);
            }

            await _repository.UpdateAsync(usuario);
            return null;
        }

        // Activar / desactivar usuario (nunca se elimina físicamente)
        public async Task ToggleActivoAsync(int id)
        {
            var usuario = await _repository.FindByIdAsync(id);
            if (usuario == null) return;

            usuario.Activo = !usuario.Activo;
            await _repository.UpdateAsync(usuario);
        }

        // Autoregistro público: siempre crea el usuario con rol Cliente y activo
        public async Task<string?> RegistrarClienteAsync(RegistroDTO dto)
        {
            if (await _repository.ExisteCorreoAsync(dto.Correo))
                return "Ya existe una cuenta registrada con ese correo.";

            string secret = _options.Value.Crypto.Secret;

            var usuario = new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                Correo = dto.Correo,
                ContrasenaHash = Cryptography.Encrypt(dto.Password, secret),
                IdRol = IdRolCliente,
                Activo = true
            };

            await _repository.AddAsync(usuario);
            return null;
        }

        // Busca clientes registrados (rol Cliente, activos) por nombre, correo o cédula
        public async Task<ICollection<ClienteBusquedaDTO>> BuscarClientesAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return new List<ClienteBusquedaDTO>();

            var usuarios = await _repository.BuscarPorRolAsync(IdRolCliente, termino.Trim());
            return usuarios.Select(u => new ClienteBusquedaDTO
            {
                IdUsuario = u.IdUsuario,
                NombreCompleto = u.NombreCompleto,
                Correo = u.Correo,
                Cedula = u.Cedula
            }).ToList();
        }
    }
}
