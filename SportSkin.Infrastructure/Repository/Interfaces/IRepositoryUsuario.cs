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
        Task<int> AddAsync(Usuario entity);
        Task UpdateAsync(Usuario entity);
        Task ChangeStateAsync(int id);
        Task<ICollection<RolUsuario>> GetRolesAsync();
        Task ChangePasswordAsync(int id, string nuevaContrasenna);

        // Validaciones de unicidad (correo y username únicos)
        Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null);
        Task<bool> ExisteUsuarioAsync(string usuario1, int? excluirId = null);

        Task<int> CountSubastasByVendedorAsync(int idUsuario);
        Task<int> CountSubastasActivasByVendedorAsync(int idUsuario);
        Task<int> CountSubastasVendidasByVendedorAsync(int idUsuario);
    }
}
