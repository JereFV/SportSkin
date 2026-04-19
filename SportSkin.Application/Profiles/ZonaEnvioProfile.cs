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
    public class ZonaEnvioProfile : Profile
    {
        public ZonaEnvioProfile()
        {
            CreateMap<ZonaEnvio, ZonaEnvioDTO>()
                .ForMember(dest => dest.Pais, opt => opt.Ignore());

            CreateMap<ZonaEnvioDTO, ZonaEnvio>()
                .ForMember(dest => dest.Pais, opt => opt.Ignore());
        }
    }
}
