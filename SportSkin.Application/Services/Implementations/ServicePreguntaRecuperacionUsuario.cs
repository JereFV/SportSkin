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
    public class ServicePreguntaRecuperacionUsuario : IServicePreguntaRecuperacionUsuario
    {
        private readonly IRepositoryPreguntaRecuperacionUsuario _repository;
        private readonly IMapper _mapper;

        public ServicePreguntaRecuperacionUsuario(IRepositoryPreguntaRecuperacionUsuario repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<PreguntaRecuperacionUsuarioDTO>> ListAsync()
        {
            var preguntas = await _repository.ListAsync();

            return _mapper.Map<ICollection<PreguntaRecuperacionUsuarioDTO>>(preguntas);
        }
    }
}
