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
    public class ServiceCondicionCamiseta : IServiceCondicionCamiseta
    {
        private readonly IRepositoryCondicionCamiseta _repositoryCamiseta;
        private readonly IMapper _mapper;

        public ServiceCondicionCamiseta(IRepositoryCondicionCamiseta repository, IMapper mapper)
        {
            _repositoryCamiseta = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<CondicionCamisetaDTO>> ListAsync()
        {
            var condicionesCamiseta = await _repositoryCamiseta.ListAsync();
            var condicionesCamisetDTO = _mapper.Map<ICollection<CondicionCamisetaDTO>>(condicionesCamiseta);

            return condicionesCamisetDTO;
        }
    }
}
