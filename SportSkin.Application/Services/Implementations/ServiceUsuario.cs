using AutoMapper;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private readonly IRepositoryUsuario _repository;
        private readonly IMapper _mapper;

        public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {
            var usuarios = await _repository.ListAsync();
            var usuariosDTO = _mapper.Map<ICollection<UsuarioDTO>>(usuarios);

            return usuariosDTO;
        }

        public async Task<UsuarioDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<UsuarioDTO>(entity);
        }
        public async Task<(int total, int activas, int vendidas, int finalizadas)> GetEstadisticasVendedorAsync(int idUsuario)
        {
            var total = await _repository.CountSubastasByVendedorAsync(idUsuario);
            var activas = await _repository.CountSubastasActivasByVendedorAsync(idUsuario);
            var vendidas = await _repository.CountSubastasVendidasByVendedorAsync(idUsuario);
            var finalizadas = total - activas - vendidas;
            return (total, activas, vendidas, finalizadas);
        }
    }
}
