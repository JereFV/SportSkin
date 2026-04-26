using Azure;
using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Interfaces
{
    public interface IServiceHistorial
    {
        Task<List<Puja>> GetPujasCompradorAsync(int idUsuario);
        Task<List<Pago>> GetPagosCompradorAsync(int idUsuario);
        Task<List<Subasta>> GetSubastasVendedorAsync(int idUsuario);

        // Pagos donde el vendedor es dueño de la camiseta subastada
        Task<List<Pago>> GetVentasVendedorAsync(int idUsuario);

        // Auxiliar para determinar si una puja fue ganadora
        Task<Dictionary<int, decimal>> GetMaxMontosFinalizadasAsync(int idUsuario);
    }
}
