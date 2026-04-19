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
    public class MetodoPagoProfile : Profile
    {
            public MetodoPagoProfile()
            {
                CreateMap<MetodoPago, MetodoPagoDTO>()
                    .ForMember(dest => dest.Factura, opt => opt.Ignore());

                CreateMap<MetodoPagoDTO, MetodoPago>()
                    .ForMember(dest => dest.Factura, opt => opt.Ignore());
            }
    }
}
