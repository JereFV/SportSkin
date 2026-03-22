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
    public class ServiceCategoriaCamiseta : IServiceCategoriaCamiseta
    {
        private readonly IRepositoryCategoriaCamiseta _repositoryCamiseta;
        private readonly IMapper _mapper;

        public ServiceCategoriaCamiseta(IRepositoryCategoriaCamiseta repository, IMapper mapper)
        {
            _repositoryCamiseta = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<CategoriaCamisetaDTO>> ListAsync()
        {
            var categorias = await _repositoryCamiseta.ListAsync();
            var categoriasDTO = _mapper.Map<ICollection<CategoriaCamisetaDTO>>(categorias);

            return categoriasDTO;
        }
    }
}
