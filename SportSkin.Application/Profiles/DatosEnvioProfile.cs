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
    public class DatosEnvioProfile : Profile
    {
        public DatosEnvioProfile()
        {
            CreateMap<DatosEnvio, DatosEnvioDTO>().ReverseMap();

            CreateMap<DatosEnvioDTO, DatosEnvio>()
                .ForMember(dest => dest.IdPaisNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdSubastaNavigation, opt => opt.Ignore());
        }
    }
}
