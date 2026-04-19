using AutoMapper;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations
{
    public class ServiceMetodoPago : IServiceMetodoPago
    {
        private readonly IRepositoryMetodoPago _repository;
        private readonly IMapper _mapper;

        public ServiceMetodoPago(IRepositoryMetodoPago repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MetodoPagoDTO>> ListAsync()
        {
            var metodos = await _repository.ListAsync();
            return _mapper.Map<ICollection<MetodoPagoDTO>>(metodos);
        }
    }
}
