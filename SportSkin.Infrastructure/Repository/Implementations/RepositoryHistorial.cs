using Azure;
using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repositories
{
    public class RepositoryHistorial : IRepositoryHistorial
    {
        private readonly SportSkinContext _context;

        public RepositoryHistorial(SportSkinContext context)
        {
            _context = context;
        }

        public async Task<List<Puja>> GetPujasByUsuarioAsync(int idUsuario)
        {
            return await _context.Puja
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.IdCamisetaNavigation)
                        .ThenInclude(c => c.ImagenCamiseta)       
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.IdEstadoSubastaNavigation)   
                .Where(p => p.IdUsuarioPuja == idUsuario)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }

        public async Task<List<Subasta>> GetSubastasByUsuarioAsync(int idUsuario)
        {
            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.ImagenCamiseta)            
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdUsuarioVendedorNavigation)    
                .Include(s => s.IdEstadoSubastaNavigation)             
                .Include(s => s.Puja)
                .Where(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();
        }

        public async Task<List<Factura>> GetPagosByUsuarioAsync(int idUsuario)
        {
            return await _context.Factura
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.IdCamisetaNavigation)
                        .ThenInclude(c => c.IdUsuarioVendedorNavigation)
                .Include(p => p.IdSubastaNavigation.IdUsuarioCompradorNavigation)
                .Include(p => p.IdEstadoFacturaNavigation)
                .Where(p => p.IdSubastaNavigation.IdUsuarioComprador == idUsuario)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        // Devuelve el monto máximo por subasta (solo subastas finalizadas)
        // donde el usuario hizo alguna puja — necesario para determinar ganador en el controller
        public async Task<Dictionary<int, int>> GetMaxMontoSubastasFinalizadasAsync(int idUsuario)
        {
            return await _context.Puja
                .Where(p => p.IdSubastaNavigation.IdEstadoSubastaNavigation.Nombre == "Vendida" && p.IdUsuarioPuja == idUsuario)
                .GroupBy(p => p.IdSubasta)
                .Select(g => new { IdSubasta = g.Key, MaxMonto = g.Max(x => x.Monto) })
                .ToDictionaryAsync(x => x.IdSubasta, x => x.MaxMonto);
        }

        public async Task<List<Factura>> GetVentasVendedorAsync(int idUsuario)
        {
            // Los Facturas donde el vendedor es dueño de la camiseta subastada
            return await _context.Factura
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.IdCamisetaNavigation)
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.IdUsuarioCompradorNavigation)
                .Include(s => s.IdEstadoFacturaNavigation)                   
                .Where(p => p.IdSubastaNavigation.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }
    }
}
