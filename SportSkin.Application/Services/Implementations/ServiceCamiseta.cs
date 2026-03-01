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
    public class ServiceCamiseta :IServiceCamiseta
    {
        private readonly IRepositoryCamiseta _repository;
        private readonly IMapper _mapper;

        public ServiceCamiseta(IRepositoryCamiseta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> AddAsync(CamisetaDTO dto)
        {
            try
            {
                var entity = _mapper.Map<Camiseta>(dto);
                return await _repository.AddAsync(entity);
            }
            catch (AutoMapperMappingException ex)
            {
                var msg = ex.ToString(); // incluye tipos origen/destino y qué miembro falló
                throw;
            }
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CamisetaDTO> FindByIdAsync(int id)
        {
            var camiseta = await _repository.FindByIdAsync(id);
            var camisetaDTO = _mapper.Map<CamisetaDTO>(camiseta);

            //foreach (subas)

            return camisetaDTO;
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByEquipo(short idEquipo)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByJugador(int idJugador)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetasAutografiadas()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetasByTemporada(short temporada)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetasByVendedor(int idUsuarioVendedor)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<CamisetaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            var collection = _mapper.Map<ICollection<CamisetaDTO>>(list);
            return collection;
        }
        public async Task<ICollection<CamisetaDTO>> GetCamisetasVendidas()
        {
            var list = await _repository.GetCamisetasVendidas();
            return _mapper.Map<ICollection<CamisetaDTO>>(list);
        }

        public async Task<ICollection<CamisetaDTO>> GetCamisetasEnSubasta()
        {
            var list = await _repository.GetCamisetasEnSubasta();
            return _mapper.Map<ICollection<CamisetaDTO>>(list);
        }

        public async Task<ICollection<CamisetaDTO>> GetCamisetasSinSubasta()
        {
            var list = await _repository.GetCamisetasSinSubasta();
            return _mapper.Map<ICollection<CamisetaDTO>>(list);
        }

        public Task UpdateAsync(int id, CamisetaDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
