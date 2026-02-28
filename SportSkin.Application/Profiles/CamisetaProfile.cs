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
                .ReverseMap();

            CreateMap<CamisetaDTO, Camiseta>()
                .ForMember(dest => dest.IdCategoriaNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.CondicionCamisetaNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.EquipoNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdJugadorNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdUsuarioVendedorNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.ImagenCamiseta, orig => orig.Ignore())
                .ForMember(dest => dest.Subasta, orig => orig.Ignore());
        }
    }
}
