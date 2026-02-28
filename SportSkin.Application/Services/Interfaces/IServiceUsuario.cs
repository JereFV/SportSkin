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
        Task<(int total, int activas, int vendidas, int finalizadas)> GetEstadisticasVendedorAsync(int idUsuario);
    }
}
