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
    public class EstadoFacturaProfile : Profile
    {
        public EstadoFacturaProfile()
        {
            CreateMap<EstadoFactura, EstadoFacturaDTO>()
                .ForMember(dest => dest.Factura, opt => opt.Ignore());

            CreateMap<EstadoFacturaDTO, EstadoFactura>()
                .ForMember(dest => dest.Factura, opt => opt.Ignore());
        }
    }
}
