using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryUsuario
    {
        Task<ICollection<Usuario>> ListAsync();
        Task<Usuario?> FindByIdAsync(int id);
        Task<int> CountSubastasByVendedorAsync(int idUsuario);
        Task<int> CountSubastasActivasByVendedorAsync(int idUsuario);
        Task<int> CountSubastasVendidasByVendedorAsync(int idUsuario);
    }
}
