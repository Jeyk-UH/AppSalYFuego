using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Profiles
{
    public class EstacionProfile : Profile
    {
        public EstacionProfile()
        {
            CreateMap<Estacion, EstacionDTO>().ReverseMap();
        }
    }
}