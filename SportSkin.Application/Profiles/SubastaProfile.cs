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
    public class SubastaProfile : Profile
    {
        public SubastaProfile()
        {
            CreateMap<Subasta, SubastaDTO>()
                .ForMember(x => x.IdEstadoSubastaNavigation, x => x.MapFrom(x => x.IdEstadoSubastaNavigation))
                .ForMember(x => x.IdCamisetaNavigation, x => x.MapFrom(x => x.IdCamisetaNavigation))
                .ReverseMap();

            CreateMap<SubastaDTO, Subasta>()
                .ForMember(dest => dest.IdCamisetaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdEstadoSubastaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuarioCompradorNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.DatosEnvio, opt => opt.Ignore())
                .ForMember(dest => dest.Factura, opt => opt.Ignore())
                .ForMember(dest => dest.Puja, opt => opt.Ignore());
        }
    }
}
