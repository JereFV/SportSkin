using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositoryPago : IRepositoryPago
    {
        private readonly SportSkinContext _context;

        public RepositoryPago(SportSkinContext context)
        {
            _context = context;
        }

        private IQueryable<Factura> QueryBase()
        {
            return _context.Factura
                .Include(f => f.IdEstadoFacturaNavigation)
                .Include(f => f.IdMetodoPagoNavigation)
                .Include(f => f.IdSubastaNavigation)
                    .ThenInclude(s => s.IdCamisetaNavigation)
                        .ThenInclude(c => c.ImagenCamiseta)
                .Include(f => f.IdSubastaNavigation)
                    .ThenInclude(s => s.IdUsuarioCompradorNavigation);
        }

        public async Task<ICollection<Factura>> ListAsync()
        {
            return await QueryBase()
                .OrderByDescending(f => f.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Factura?> FindByIdAsync(string idFactura)
        {
            return await QueryBase()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdFactura == idFactura);
        }

        public async Task<Factura?> FindBySubastaAsync(int idSubasta)
        {
            return await _context.Factura
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdSubasta == idSubasta);
        }

        public async Task<string> AddAsync(Factura factura)
        {
            _context.Factura.Add(factura);
            await _context.SaveChangesAsync();
            return factura.IdFactura;
        }

        public async Task ConfirmarAsync(string idFactura)
        {
            await _context.Factura
                .Where(f => f.IdFactura == idFactura)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(f => f.IdEstadoFactura, (byte)2)
                    .SetProperty(f => f.FechaPago, DateTime.Now));
        }

        // Subastas con ganador determinado que aún NO tienen factura
        public async Task<ICollection<Subasta>> GetSubastasPendientesPagoAsync()
        {
            var idsConFactura = await _context.Factura
                .Where(f => f.IdSubasta != null)
                .Select(f => f.IdSubasta!.Value)
                .ToListAsync();

            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.ImagenCamiseta)
                .Include(s => s.IdUsuarioCompradorNavigation)
                .Where(s => s.IdUsuarioComprador != null
                         && s.MontoCompra != null
                         && !idsConFactura.Contains(s.IdSubasta))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}