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
    public class FacturaProfile : Profile
    {
        public FacturaProfile()
        {
            CreateMap<Factura, FacturaDTO>().ReverseMap();

            CreateMap<FacturaDTO, Factura>()
                .ForMember(dest => dest.IdEstadoFacturaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdMetodoPagoNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdSubastaNavigation, opt => opt.Ignore());
        }
    }
}
