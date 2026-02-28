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
    public class ServiceSubasta : IServiceSubasta
    {
        private readonly IRepositorySubasta _repository;
        private readonly IMapper _mapper;

        public ServiceSubasta(IRepositorySubasta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<SubastaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<SubastaDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<SubastaDTO>(entity);
        }

        public async Task<int> AddAsync(SubastaDTO dto)
        {
            try
            {
                var entity = _mapper.Map<Subasta>(dto);
                return await _repository.AddAsync(entity);
            }
            catch (AutoMapperMappingException ex)
            {
                var msg = ex.ToString();
                throw;
            }
        }

        public async Task UpdateAsync(int id, SubastaDTO dto)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"No existe la subasta con id={id}");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<ICollection<SubastaDTO>> GetSubastasActivasAsync(
        DateTime? desde, DateTime? hasta)
        {
            var list = await _repository.GetSubastasActivasAsync(desde, hasta);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> GetSubastasFinalizadasAsync(
        DateTime? desde, DateTime? hasta)
        {
            var list = await _repository.GetSubastasFinalizadasAsync(desde, hasta);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> GetSubastasVendidasAsync()
        {
            var list = await _repository.GetSubastasVendidasAsync();
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }
        public async Task<ICollection<SubastaDTO>> GetSubastasByVendedorAsync(int idUsuarioVendedor)
        {
            var list = await _repository.GetSubastasByVendedorAsync(idUsuarioVendedor);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }
    }
}
