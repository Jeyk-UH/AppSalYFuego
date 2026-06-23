using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Profiles
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            // Mapeo de MenuDisponibilidad a MenuDisponibilidadDTO
            CreateMap<MenuDisponibilidad, MenuDisponibilidadDTO>().ReverseMap();

            // Mapeo de MenuItem a MenuItemDTO
            CreateMap<MenuItem, MenuItemDTO>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null
                        ? src.IdProductoNavigation.Nombre
                        : src.IdComboNavigation.Nombre))
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null
                        ? src.IdProductoNavigation.Precio
                        : src.IdComboNavigation.PrecioEspecial))
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null
                        ? src.IdProductoNavigation.ProductoImagen
                            .Where(i => i.EsPrincipal == true)
                            .Select(i => i.UrlImagen)
                            .FirstOrDefault()
                        : src.IdComboNavigation.UrlImagen))
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null
                        ? src.IdProductoNavigation.IdCategoriaNavigation.Nombre
                        : src.IdComboNavigation.IdCategoriaNavigation.Nombre))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null ? "Producto" : "Combo"))
                .ReverseMap();

            // Mapeo de Menu a MenuDTO
            CreateMap<Menu, MenuDTO>()
                .ForMember(dest => dest.Disponibilidad,
                    opt => opt.MapFrom(src => src.MenuDisponibilidad))
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.MenuItem))
                .ReverseMap();
        }
    }
}