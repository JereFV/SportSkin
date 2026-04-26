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
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(x => x.RolUsuarioNavigation, x => x.MapFrom(x => x.IdRolUsuarioNavigation))
                .ForMember(x => x.Camiseta, x => x.MapFrom(x => x.Camiseta))
                .ForMember(x => x.Puja, x => x.MapFrom(x => x.Puja))
                .ForMember(x => x.Subasta, x => x.MapFrom(x => x.Subasta))
                .ForMember(x => x.PreguntaRecuperacionNavigation, x => x.MapFrom(x => x.IdPreguntaRecuperacionNavigation))
                .ReverseMap();

            CreateMap<UsuarioDTO, Usuario>()
            .ForMember(dest => dest.IdRolUsuarioNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.Camiseta, opt => opt.Ignore())
            .ForMember(dest => dest.Puja, opt => opt.Ignore())
            .ForMember(dest => dest.Subasta, opt => opt.Ignore())
            .ForMember(dest => dest.IdPreguntaRecuperacionNavigation, opt => opt.Ignore());
        }
    }
    
}
