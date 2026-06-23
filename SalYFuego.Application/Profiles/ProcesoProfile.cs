using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Profiles
{
    public class ProcesoProfile : Profile
    {
        public ProcesoProfile()
        {
            // Mapeo de ProcesoPreparacion a PasoProcesoDTO
            CreateMap<ProcesoPreparacion, PasoProcesoDTO>()
                .ForMember(dest => dest.NombreEstacion,
                    opt => opt.MapFrom(src => src.IdEstacionNavigation.Nombre))
                .ReverseMap();

            // Mapeo de Producto a ProcesoDTO
            CreateMap<Producto, ProcesoDTO>()
                .ForMember(dest => dest.NombreProducto,
                    opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.CantidadPasos,
                    opt => opt.MapFrom(src => src.ProcesoPreparacion.Count))
                .ForMember(dest => dest.Pasos,
                    opt => opt.MapFrom(src => src.ProcesoPreparacion
                        .OrderBy(p => p.OrdenPaso)))
                .ReverseMap();
        }
    }
}