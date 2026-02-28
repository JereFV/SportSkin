using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositoryCamiseta : IRepositoryCamiseta
    {

        private readonly SportSkinContext _context;

        public RepositoryCamiseta(SportSkinContext context)
        {
            _context = context;

        }

        public Task<int> AddAsync(Camiseta entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Camiseta?> FindByIdAsync(int id)
        {
            var camiseta = await _context.Set<Camiseta>()
                                   .Where(x => x.IdCamiseta == id)
                                   .Include(x => x.ImagenCamiseta)
                                   .Include(x => x.IdCondicionCamisetaNavigation)
                                   .Include(x => x.IdEstadoCamisetaNavigation)
                                   .Include(x => x.IdEquipoNavigation)
                                   .Include(x => x.IdJugadorNavigation)
                                   .Include(x => x.IdCategoriaCamiseta)
                                   .Include(x => x.IdUsuarioVendedorNavigation)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync();

            return camiseta;
        }

        public Task<ICollection<Camiseta>> GetCamisetaByCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Camiseta>> GetCamisetaByEquipo(int idEquipo)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Camiseta>> GetCamisetaByJugador(int idJugador)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Camiseta>> GetCamisetaByNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Camiseta>> GetCamisetasAutografiadas()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Camiseta>> GetCamisetasByTemporada(short temporada)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Camiseta>> GetCamisetasByVendedor(int idUsuarioVendedor)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Camiseta>> ListAsync()
        {
            var collection = await _context.Set<Camiseta>()
                .Include(c => c.IdCondicionCamisetaNavigation)
                .Include(c => c.IdEquipoNavigation)
                .Include(c => c.IdJugadorNavigation)
                .Include(c => c.ImagenCamiseta)
                .Include(c => c.EstadoCamisetaNavigation)              
                .OrderBy(c => c.IdCamiseta)
                .AsNoTracking()
                .ToListAsync();

            return collection;
        }

        public async Task<ICollection<Camiseta>> GetCamisetasVendidas()
        {
            return await _context.Camiseta
                .Include(c => c.IdCondicionCamisetaNavigation)
                .Include(c => c.IdEquipoNavigation)
                .Include(c => c.IdJugadorNavigation)
                .Include(c => c.ImagenCamiseta)
                .Include(c => c.IdEstadoCamisetaNavigation) 
                .Where(c => c.IdEstadoCamiseta == 3         // 3 = Vendida
                         && c.EstadoRegistro == true)        // solo registros activos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Camiseta>> GetCamisetasEnSubasta()
        {
            return await _context.Camiseta
                .Include(c => c.IdCondicionCamisetaNavigation)
                .Include(c => c.IdEquipoNavigation)
                .Include(c => c.IdJugadorNavigation)
                .Include(c => c.ImagenCamiseta)
                .Include(c => c.IdEstadoCamisetaNavigation)
                .Where(c => c.IdEstadoCamiseta == 2         // 2 = En Subasta
                         && c.EstadoRegistro == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Camiseta>> GetCamisetasSinSubasta()
        {
            return await _context.Camiseta
                .Include(c => c.IdCondicionCamisetaNavigation)
                .Include(c => c.IdEquipoNavigation)
                .Include(c => c.IdJugadorNavigation)
                .Include(c => c.ImagenCamiseta)
                .Include(c => c.IdEstadoCamisetaNavigation)
                .Where(c => c.IdEstadoCamiseta == 1         // 1 = Disponible
                         && c.EstadoRegistro == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task UpdateAsync(Camiseta entity)
        {
            throw new NotImplementedException();
        }
    }
}
