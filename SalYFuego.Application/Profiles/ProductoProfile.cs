using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Profiles
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            // Mapeo de Ingrediente a IngredienteDTO
            CreateMap<Ingrediente, IngredienteDTO>().ReverseMap();

            // Mapeo de Producto a ProductoDTO
            CreateMap<Producto, ProductoDTO>()
                .ForMember(dest => dest.CategoriaNombre,
                    opt => opt.MapFrom(src => src.IdCategoriaNavigation.Nombre))
                .ForMember(dest => dest.Ingredientes,
                    opt => opt.MapFrom(src => src.IdIngrediente))
                .ForMember(dest => dest.IdCategoria,
    opt => opt.MapFrom(src => src.IdCategoria))
                .ForMember(dest => dest.ImagenUrl,
                    opt => opt.MapFrom(src => src.ProductoImagen
                        .Where(i => i.EsPrincipal == true)
                        .Select(i => i.UrlImagen)
                        .FirstOrDefault()))
                .ReverseMap();
        }
    }
}