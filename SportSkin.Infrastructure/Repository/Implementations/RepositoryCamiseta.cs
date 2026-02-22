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

        public Task<Camiseta> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<ICollection<Camiseta>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Camiseta entity)
        {
            throw new NotImplementedException();
        }
    }
}
