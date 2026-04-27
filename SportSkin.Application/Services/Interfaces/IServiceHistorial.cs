using Azure;
using SportSkin.Application.DTOs;
using SportSkin.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportSkin.Core.Interfaces
{
    public interface IServiceHistorial
    {
        Task<List<PujaDTO>> GetPujasCompradorAsync(int idUsuario);
        Task<List<FacturaDTO>> GetFacturasCompradorAsync(int idUsuario);
        Task<List<SubastaDTO>> GetSubastasVendedorAsync(int idUsuario);

        // Facturas donde el vendedor es dueño de la camiseta subastada
        Task<List<FacturaDTO>> GetVentasVendedorAsync(int idUsuario);

        // Auxiliar para determinar si una puja fue ganadora
        Task<Dictionary<int, int>> GetMaxMontosFinalizadasAsync(int idUsuario);
    }
}
