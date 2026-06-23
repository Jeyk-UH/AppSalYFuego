using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Profiles
{
    public class ComboProfile : Profile
    {
        public ComboProfile()
        {
            // Mapeo de ComboProducto a ComboProductoDTO
            CreateMap<ComboProducto, ComboProductoDTO>()
                .ForMember(dest => dest.NombreProducto,
                    opt => opt.MapFrom(src => src.IdProductoNavigation.Nombre))
                .ForMember(dest => dest.IdProducto,
                    opt => opt.MapFrom(src => src.IdProducto))
                .ReverseMap();

            // Mapeo de Combo a ComboDTO
            CreateMap<Combo, ComboDTO>()
                .ForMember(dest => dest.CategoriaNombre,
                    opt => opt.MapFrom(src => src.IdCategoriaNavigation.Nombre))
                .ForMember(dest => dest.Productos,
                    opt => opt.MapFrom(src => src.ComboProducto))
                .ReverseMap();
        }
    }
}