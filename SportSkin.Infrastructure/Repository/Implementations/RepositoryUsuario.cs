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
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly SportSkinContext _context;

        public RepositoryUsuario(SportSkinContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Usuario>> ListAsync()
        {
            var usuarios = await _context.Set<Usuario>()
                                         .Include(x => x.IdRolUsuarioNavigation)
                                         .AsNoTracking()
                                         .ToListAsync();

            return usuarios;
        }
        public async Task<Usuario?> FindByIdAsync(int id)
        {
            return await _context.Usuario
                .Include(u => u.IdRolUsuarioNavigation)
                .Include(u => u.Camiseta)
                    .ThenInclude(c => c.Subasta)
                .Include(u => u.Puja)
                .Include(u => u.Subasta)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }
        public async Task<int> CountSubastasByVendedorAsync(int idUsuario)
        {
            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                .CountAsync(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario);
        }

        public async Task<int> CountSubastasActivasByVendedorAsync(int idUsuario)
        {
            var fechaActual = DateTime.Now;
            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                .CountAsync(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario
                              && s.FechaCompra == null
                              && s.FechaCierre > fechaActual);
        }

        public async Task<int> CountSubastasVendidasByVendedorAsync(int idUsuario)
        {
            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                .CountAsync(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario
                              && s.FechaCompra != null);
        }
    }
}
