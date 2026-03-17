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
    internal static class EstadoSubastaIds
    {
        public const byte EnProceso = 1;
        public const byte Cerrada = 2;
        public const byte Vendida = 3;
        public const byte Finalizada = 4;
        public const byte Borrador = 5;
    }
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

        /*
            Crea subasta con estado Borrador(5).
            Se verifica FechaCierre > FechaInicio, camiseta sin subasta activa.
        */
        public async Task<int> AddAsync(SubastaDTO dto)
        {
            if (dto.FechaCierre <= dto.FechaInicio)
                throw new ArgumentException(
                    "La fecha de cierre debe ser posterior a la fecha de inicio.");

            if (dto.PrecioBase <= 0)
                throw new InvalidOperationException("El precio base debe ser mayor a 0.");

            if (dto.IncrementoMinimo <= 0)
                throw new InvalidOperationException("El incremento mínimo debe ser mayor a 0.");

            bool tieneActiva = await _repository.CamisetaTieneSubastaActivaAsync(dto.IdCamiseta);
            if (tieneActiva)
                throw new InvalidOperationException(
                    "La camiseta seleccionada ya tiene una subasta activa (En proceso).");

            // Estado inicial: Borrador
            dto = dto with { IdEstadoSubasta = EstadoSubastaIds.Borrador };

            var entity = _mapper.Map<Subasta>(dto);
            return await _repository.AddAsync(entity);
        }

        /*
            Edita subasta. Solo si no ha iniciado y no tiene pujas.
        */
        public async Task UpdateAsync(int id, SubastaDTO dto)
        {
            var entity = await _repository.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"No existe la subasta con id={id}");

            if (!await PuedeEditarAsync(id))
                throw new InvalidOperationException("La subasta no puede editarse: ya inició o tiene pujas.");

            if (dto.FechaCierre <= dto.FechaInicio)
                throw new ArgumentException(
                    "La fecha de cierre debe ser posterior a la fecha de inicio.");
            if (dto.PrecioBase <= 0)
                throw new InvalidOperationException("El precio base debe ser mayor a 0.");

            if (dto.IncrementoMinimo <= 0)
                throw new InvalidOperationException("El incremento mínimo debe ser mayor a 0.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }


        // Publica manualmente: Borrador(5) → En proceso(1)
        public async Task PublicarAsync(int id)
        {
            await _repository.PublicarAsync(id);
        }

        //Cancela manualmente → Cerrada(2)
        public async Task CancelarAsync(int id)
        {
            await _repository.CancelarAsync(id);
        }

       
        // Puede editarse si NO ha iniciado (FechaInicio > Now) Y NO tiene pujas.
       
        public async Task<bool> PuedeEditarAsync(int id)
        {
            var subasta = await _repository.FindByIdAsync(id);
            if (subasta is null) return false;

            bool noHaIniciado = subasta.FechaInicio > DateTime.Now;
            bool sinPujas = !subasta.Puja.Any();

            return noHaIniciado && sinPujas;
        }

        // Puede cancelarse si NO ha iniciado O no tiene pujas.
        public async Task<bool> PuedeCancelarAsync(int id)
        {
            var subasta = await _repository.FindByIdAsync(id);
            if (subasta is null) return false;

            bool noHaIniciado = subasta.FechaInicio > DateTime.Now;
            bool sinPujas = !subasta.Puja.Any();

            return noHaIniciado || sinPujas;
        }

        public Task<bool> CamisetaTieneSubastaActivaAsync(int idCamiseta, int? excludeIdSubasta = null)
        => _repository.CamisetaTieneSubastaActivaAsync(idCamiseta, excludeIdSubasta);

        /*
            Llamado por el Background Service.
            Borrador(5) → En proceso(1) cuando FechaInicio ya llegó.
        */
        /*
        public async Task<int> ActivarSubastasPendientesAsync()
        {
            return await _repository.ActivarSubastasPendientesAsync();
        }
        */

        /*
            Llamado por el Background Service.
            En proceso(1) → Finalizada(4) o Cerrada(2) cuando FechaCierre ya pasó.
        */
        /*
        public async Task<int> CerrarSubastasVencidasAsync()
        {
            return await _repository.CerrarSubastasVencidasAsync();
        }
        */

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

        public async Task<ICollection<SubastaDTO>> GetSubastasMasPopularesAsync(int top)
        {
            var list = await _repository.GetSubastasMasPopularesAsync(top);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }
    }
}
