using AutoMapper;
using SportSkin.Application.DTOs;
using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Profiles
{
    public class CamisetaProfile : Profile
    {
        public CamisetaProfile()
        {
            CreateMap<Camiseta, CamisetaDTO>()
                .ForMember(x => x.ImagenesCamiseta, x => x.MapFrom(x => x.ImagenCamiseta))
                .ForMember(x => x.CondicionCamisetaNavigation, x => x.MapFrom(x => x.IdCondicionCamisetaNavigation))
                .ForMember(x => x.EstadoCamisetaNavigation, x => x.MapFrom(x => x.IdEstadoCamisetaNavigation))
                .ForMember(x => x.EquipoNavigation, x => x.MapFrom(x => x.IdEquipoNavigation))
                .ForMember(x => x.JugadorNavigation, x => x.MapFrom(x => x.IdJugadorNavigation))
                .ForMember(x => x.CategoriasCamiseta, x => x.MapFrom(x => x.IdCategoriaCamiseta))
                .ForMember(x => x.UsuarioVendedorNavigation, x => x.MapFrom(x => x.IdUsuarioVendedorNavigation))
                .ForMember(x => x.Subastas, x => x.MapFrom(x => x.Subasta))                
                .ReverseMap();

            //DTO -> Entidad (Crear, Editar)
            CreateMap<CamisetaDTO, Camiseta>()
                .ForMember(x => x.IdCategoriaCamiseta, x => x.MapFrom(x => x.CategoriasCamiseta))
                .ForMember(x => x.IdEquipoNavigation, x => x.MapFrom(x => x.EquipoNavigation))
                .ForMember(x => x.IdJugadorNavigation, x => x.MapFrom(x => x.JugadorNavigation))
                .ForMember(dest => dest.IdCondicionCamisetaNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdEstadoCamisetaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuarioVendedorNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.ImagenCamiseta, orig => orig.Ignore())
                .ForMember(dest => dest.Subasta, orig => orig.Ignore());                       
        }
    }
}
