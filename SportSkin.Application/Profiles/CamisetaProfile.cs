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
            CreateMap<Camiseta, CamisetaDTO>().ReverseMap();

            CreateMap<CamisetaDTO, Camiseta>()
                .ForMember(dest => dest.IdCategoriaNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdCondicionCamisetaNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdEquipoNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdJugadorNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.IdUsuarioVendedorNavigation, orig => orig.Ignore())
                .ForMember(dest => dest.ImagenCamiseta, orig => orig.Ignore())
                .ForMember(dest => dest.Subasta, orig => orig.Ignore());
        }
    }
}
