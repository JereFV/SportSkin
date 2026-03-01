using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Interfaces
{
    public interface IRepositorySubasta
    {
        Task<ICollection<Subasta>> ListAsync();
        Task<Subasta?> FindByIdAsync(int id);
        Task<int> AddAsync(Subasta entity);
        Task UpdateAsync(Subasta entity);
        Task DeleteAsync(int id);

        // Filtros
        Task<ICollection<Subasta>> GetSubastasActivasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<Subasta>> GetSubastasFinalizadasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<Subasta>> GetSubastasVendidasAsync();
        Task<ICollection<Subasta>> GetSubastasByVendedorAsync(int idUsuarioVendedor);
    }
}
