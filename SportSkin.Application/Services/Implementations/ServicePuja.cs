using AutoMapper;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations
{
    public class ServicePuja : IServicePuja
    {
        private readonly IRepositoryPuja _repository;
        private readonly IMapper _mapper;

        public ServicePuja(IRepositoryPuja repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task AddAsync(PujaDTO dto)
        {         
            var puja = _mapper.Map<Puja>(dto);

            await _repository.AddAsync(puja);
        }
    }
}
