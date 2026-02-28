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
                .ForMember(x => x.ImagenCamiseta, x => x.MapFrom(x => x.ImagenCamiseta))
                .ForMember(x => x.CondicionCamisetaNavigation, x => x.MapFrom(x => x.CondicionCamisetaNavigation))
                .ForMember(x => x.EstadoCamisetaNavigation, x => x.MapFrom(x => x.EstadoCamisetaNavigation))
                .ForMember(x => x.EquipoNavigation, x => x.MapFrom(x => x.EquipoNavigation))
                .ForMember(x => x.IdJugadorNavigation, x => x.MapFrom(x => x.IdJugadorNavigation))
                .ForMember(x => x.IdUsuarioVendedorNavigation, x => x.MapFrom(x => x.IdUsuarioVendedorNavigation))
                .ForMember(x => x.IdCategoriaNavigation, x => x.MapFrom(x => x.IdCategoriaNavigation))
                .ForMember(x => x.Subasta, x => x.Ignore())
                .ReverseMap();

            CreateMap<CamisetaDTO, Camiseta>()
                .ForMember(dest => dest.IdCategoriaNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.CondicionCamisetaNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.EquipoNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.EstadoCamisetaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdJugadorNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdUsuarioVendedorNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.ImagenCamiseta, orig => orig.Ignore())
                .ForMember(dest => dest.Subasta, orig => orig.Ignore());
        }
    }
}
