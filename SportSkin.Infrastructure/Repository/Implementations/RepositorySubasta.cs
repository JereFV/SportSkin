using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositorySubasta : IRepositorySubasta
    {
        private readonly SportSkinContext _context;

        public RepositorySubasta(SportSkinContext context)
        {
            _context = context;
        }

        // Includes base 
        private IQueryable<Subasta> QueryBase()
        {
            return _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.ImagenCamiseta)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdCondicionCamisetaNavigation)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdEquipoNavigation)             
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdEstadoCamisetaNavigation)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdJugadorNavigation)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdCategoriaCamiseta)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                    .ThenInclude(c => c.IdUsuarioPujaNavigation);
        }

        public async Task<ICollection<Subasta>> ListAsync()
        {
            return await QueryBase()
                .OrderByDescending(s => s.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Subasta?> FindByIdAsync(int id)
        {
            return await QueryBase()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSubasta == id);
        }

        public async Task<int> AddAsync(Subasta entity)
        {
            entity.FechaInicio = DateTime.Now;
            _context.Subasta.Add(entity);
            await _context.SaveChangesAsync();
            return entity.IdSubasta;
        }

        public async Task UpdateAsync(Subasta entity)
        {
            _context.Subasta.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Subasta.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"No existe la subasta con id={id}");

            _context.Subasta.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // Activas — sin FechaCompra y FechaCierre futura
        public async Task<ICollection<Subasta>> GetSubastasActivasAsync(
        DateTime? desde, DateTime? hasta)
        {
            //Solución vuelta loca
            //var fechaActual = DateTime.ParseExact(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture), ("yyyy/MM/dd HH:mm:ss.fff"), CultureInfo.InvariantCulture);
            var fechaPrueba = DateTime.Now;
            var query = QueryBase()
                .Where(s => s.FechaCompra == null
                         && s.FechaCierre > fechaPrueba);

            // Aplica filtro de fecha solo si se proporcionó
            if (desde.HasValue)
                query = query.Where(s => s.FechaInicio >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(s => s.FechaInicio <= hasta.Value);

            return await query
                .OrderByDescending(s => s.FechaCierre)
                .AsNoTracking()
                .ToListAsync();
        }

        // Finalizadas — FechaCierre pasada pero sin compra
        public async Task<ICollection<Subasta>> GetSubastasFinalizadasAsync(
        DateTime? desde, DateTime? hasta)
        {
            var fechaActual = DateTime.Now;
            var query = QueryBase()
                .Where(s => s.FechaCierre <= fechaActual);

            if (desde.HasValue)
                query = query.Where(s => s.FechaCierre >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(s => s.FechaCierre <= hasta.Value);

            return await query
                .OrderByDescending(s => s.FechaCierre)
                .AsNoTracking()
                .ToListAsync();
        }

        // Vendidas — tienen FechaCompra registrada
        public async Task<ICollection<Subasta>> GetSubastasVendidasAsync()
        {
            return await QueryBase()
                .Where(s => s.FechaCompra != null)
                .OrderByDescending(s => s.FechaCompra)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Subasta>> GetSubastasByVendedorAsync(int idUsuarioVendedor)
        {
            return await QueryBase()
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdUsuarioVendedorNavigation)
                .Where(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuarioVendedor)
                .OrderByDescending(s => s.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        // Subastas activas ordenadas por cantidad de pujas
        public async Task<ICollection<Subasta>> GetSubastasMasPopularesAsync(int top)
        {
            var fechaActual = DateTime.Now;
            return await QueryBase()
                .Where(s => s.FechaCompra == null && s.FechaCierre > fechaActual)
                .OrderByDescending(s => s.Puja.Count)
                .Take(top)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
