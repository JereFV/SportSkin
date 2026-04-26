using Azure;
using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryHistorial
    {
        Task<List<Puja>> GetPujasByUsuarioAsync(int idUsuario);
        Task<List<Subasta>> GetSubastasByUsuarioAsync(int idUsuario);
        Task<List<Pago>> GetPagosByUsuarioAsync(int idUsuario);

        // Para el cálculo de ganador: monto máximo por subasta finalizada
        Task<Dictionary<int, decimal>> GetMaxMontoSubastasFinalizadasAsync(int idUsuario);
    }
}
