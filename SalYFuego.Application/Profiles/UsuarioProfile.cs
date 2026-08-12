using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Profiles
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            // Mapeo de Rol a RolDTO
            CreateMap<Rol, RolDTO>();

            // Mapeo de Usuario a UsuarioDTO (listado / datos de sesión)
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.NombreRol,
                    opt => opt.MapFrom(src => src.IdRolNavigation.NombreRol));

            // Mapeo de Usuario a UsuarioFormDTO (para precargar el formulario de edición)
            // La contraseña nunca se trae de vuelta al formulario
            CreateMap<Usuario, UsuarioFormDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
        }
    }
}
