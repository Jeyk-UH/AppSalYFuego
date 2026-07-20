using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Profiles
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        }
    }
}