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
            // Entidad → DTO: mapea navegaciones
            CreateMap<Factura, FacturaDTO>()
                .ForMember(dest => dest.IdEstadoFacturaNavigation,
                           opt => opt.MapFrom(src => src.IdEstadoFacturaNavigation))
                .ForMember(dest => dest.IdMetodoPagoNavigation,
                           opt => opt.MapFrom(src => src.IdMetodoPagoNavigation))
                .ForMember(dest => dest.IdSubastaNavigation,
                           opt => opt.MapFrom(src => src.IdSubastaNavigation));

            // DTO → Entidad: ignora navegaciones (solo se usan FKs escalares)
            CreateMap<FacturaDTO, Factura>()
                .ForMember(dest => dest.IdEstadoFacturaNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdMetodoPagoNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdSubastaNavigation, opt => opt.Ignore());
        }
    }
}
