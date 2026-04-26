using SportSkin.Application.DTOs;
using SportSkin.Application.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Interfaces
{
    public interface IServiceUsuario
    {
        Task<ICollection<UsuarioDTO>> ListAsync();
        Task<UsuarioDTO> FindByIdAsync(int id);
        Task<int> AddAsync(UsuarioDTO dto);
        Task UpdateAsync(int id, UsuarioDTO dto);
        Task ChangeStateAsync(int id);
        Task ChangePasswordAsync(int id, string nuevaContrasenna);
        Task<ICollection<RolUsuarioDTO>> GetRolesAsync();
        Task<(int total, int activas, int vendidas, int finalizadas)> GetEstadisticasVendedorAsync(int idUsuario);
        Task<UsuarioDTO?> LoginAsync(string user, string password);
        Task<UsuarioDTO> FindByUserAsync(string usuario);
    }
}
