using Azure;
using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositoryHistorial : IRepositoryHistorial
    {
        private readonly SportSkinContext _context;
        private readonly IRepositoryHistorial _repo;
        public RepositoryHistorial(IRepositoryHistorial repo, SportSkinContext context)
        {
            _repo = repo;
            _context = context;
        }

        public Task<List<Puja>> GetPujasCompradorAsync(int idUsuario)
            => _repo.GetPujasByUsuarioAsync(idUsuario);

        public Task<List<Pago>> GetPagosCompradorAsync(int idUsuario)
            => _repo.GetPagosByUsuarioAsync(idUsuario);

        public Task<List<Subasta>> GetSubastasVendedorAsync(int idUsuario)
            => _repo.GetSubastasByUsuarioAsync(idUsuario);

        public async Task<List<Pago>> GetVentasVendedorAsync(int idUsuario)
        {
            // Los pagos donde el vendedor es dueño de la camiseta subastada
            return await _context.Pagos
                .Include(p => p.Subasta)
                    .ThenInclude(s => s.Camiseta)
                .Include(p => p.UsuarioComprador)
                .Where(p => p.Subasta.Camiseta.IdUsuario == idUsuario)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        public Task<Dictionary<int, decimal>> GetMaxMontosFinalizadasAsync(int idUsuario)
            => _repo.GetMaxMontoSubastasFinalizadasAsync(idUsuario);
    }
}
