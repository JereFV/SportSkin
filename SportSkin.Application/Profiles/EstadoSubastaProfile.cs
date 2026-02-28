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
    public class EstadoSubastaProfile : Profile
    {
        public EstadoSubastaProfile()
        {
            CreateMap<EstadoSubasta, EstadoSubastaDTO>().ReverseMap();
        }
    }
}
