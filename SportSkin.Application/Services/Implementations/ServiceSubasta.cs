using AutoMapper;
using Microsoft.Extensions.Options;
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
        public const byte Vendida = 2;
        public const byte Finalizada = 3;
        public const byte Borrador = 4;
        public const byte Cancelada = 5;
        public const byte Publicada = 6;
    }
    public class ServiceSubasta : IServiceSubasta
    {
        private readonly IRepositorySubasta _repositorySubasta;
        private readonly IMapper _mapper;
        private readonly SubastaSettings _settings;
        public ServiceSubasta(IRepositorySubasta repository, IMapper mapper, IOptions<SubastaSettings> settings)
        {
            _repositorySubasta = repository;
            _mapper = mapper;
            _settings = settings.Value;
        }

        public async Task<ICollection<SubastaDTO>> ListAsync()
        {
            var list = await _repositorySubasta.ListAsync();
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<SubastaDTO> FindByIdAsync(int id)
        {
            var entity = await _repositorySubasta.FindByIdAsync(id);
            return _mapper.Map<SubastaDTO>(entity);
        }

        /*
            Crea subasta con estado Borrador(5).
            Se verifica FechaCierre > FechaInicio, Subasta sin subasta activa.
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

            bool bloqueada = await _repositorySubasta.CamisetaTieneSubastaActivaAsync(dto.IdCamiseta);
            if (bloqueada)
                throw new InvalidOperationException(
                    "La camiseta ya tiene una subasta activa, vendida o en borrador. " +
                    "Resolvé el estado actual antes de crear una nueva.");

            // Estado inicial: Borrador
            dto = dto with { IdEstadoSubasta = EstadoSubastaIds.Borrador,
                             PorcentajeComision = _settings.PorcentajeComision
            };

            var entity = _mapper.Map<Subasta>(dto);
            return await _repositorySubasta.AddAsync(entity);
        }

        /*
            Edita subasta. Solo si no ha iniciado y no tiene pujas.
        */
        public async Task UpdateAsync(int id, SubastaDTO dto)
        {
            var entity = await _repositorySubasta.FindByIdAsync(id)
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
            entity.PorcentajeComision = _settings.PorcentajeComision;
            await _repositorySubasta.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repositorySubasta.DeleteAsync(id);
        }


        // Publica manualmente: Borrador(5) → En proceso(1)
        public async Task PublicarAsync(int id)
        {
            await _repositorySubasta.PublicarAsync(id);
        }

        //Cancela manualmente → Cerrada(2)
        public async Task CancelarAsync(int id)
        {
            await _repositorySubasta.CancelarAsync(id);
        }

       
        // Puede editarse si NO ha iniciado (FechaInicio > Now) Y NO tiene pujas.
       
        public async Task<bool> PuedeEditarAsync(int id)
        {
            var subasta = await _repositorySubasta.FindByIdAsync(id);
            if (subasta is null) return false;

            bool noHaIniciado = subasta.FechaInicio > DateTime.Now;
            bool sinPujas = !subasta.Puja.Any();

            return noHaIniciado && sinPujas;
        }

        // Puede cancelarse si NO ha iniciado O no tiene pujas.
        public async Task<bool> PuedeCancelarAsync(int id)
        {
            var subasta = await _repositorySubasta.FindByIdAsync(id);
            if (subasta is null) return false;

            bool noHaIniciado = subasta.FechaInicio > DateTime.Now;
            bool sinPujas = !subasta.Puja.Any();

            return noHaIniciado || sinPujas;
        }

        public Task<bool> CamisetaTieneSubastaActivaAsync(int idCamiseta, int? excludeIdSubasta = null)
        => _repositorySubasta.CamisetaTieneSubastaActivaAsync(idCamiseta, excludeIdSubasta);

        /*
            Llamado por el Background Service.
            Borrador(5) → En proceso(1) cuando FechaInicio ya llegó.
        */

        public async Task<int> ActivarSubastasPendientesAsync()
           => await _repositorySubasta.ActivarSubastasPendientesAsync();

        // Cierra y devuelve solo el conteo al Background Service.
        // El BG Service accede al repo directamente para obtener la lista
        // y notificar por SignalR (ver SubastaBackgroundService).
        public async Task<int> CerrarSubastasVencidasAsync()
        {
            var cerradas = await _repositorySubasta.CerrarSubastasVencidasAsync();
            return cerradas.Count;
        }


        public async Task<ICollection<SubastaDTO>> GetSubastasActivasAsync(
        DateTime? desde, DateTime? hasta)
        {
            var list = await _repositorySubasta.GetSubastasActivasAsync(desde, hasta);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> GetSubastasFinalizadasAsync(
        DateTime? desde, DateTime? hasta)
        {
            var list = await _repositorySubasta.GetSubastasFinalizadasAsync(desde, hasta);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> GetSubastasVendidasAsync()
        {
            var list = await _repositorySubasta.GetSubastasVendidasAsync();
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }
        public async Task<ICollection<SubastaDTO>> GetSubastasByVendedorAsync(int idUsuarioVendedor)
        {
            var list = await _repositorySubasta.GetSubastasByVendedorAsync(idUsuarioVendedor);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> GetSubastasMasPopularesAsync(int top)
        {
            var list = await _repositorySubasta.GetSubastasMasPopularesAsync(top);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

       

    }
}
