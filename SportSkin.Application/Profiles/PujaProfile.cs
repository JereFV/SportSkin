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
    public class PujaProfile : Profile
    {
        public PujaProfile()
        {
            CreateMap<Puja, PujaDTO>().ReverseMap();

            CreateMap<PujaDTO, Puja>()
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IdSubastaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuarioPujaNavigation, opt => opt.Ignore());
        }
    }
}
