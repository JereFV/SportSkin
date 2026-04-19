using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryPago
    {
        Task<ICollection<Factura>> ListAsync();
        Task<Factura?> FindByIdAsync(string idFactura);
        Task<Factura?> FindBySubastaAsync(int idSubasta);
        Task<string> AddAsync(Factura factura);
        Task ConfirmarAsync(string idFactura);
        Task<ICollection<Subasta>> GetSubastasPendientesPagoAsync();
    }
}
