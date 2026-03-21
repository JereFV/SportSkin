using SportSkin.Application.DTOs;
using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Interfaces
{
    public interface IServiceCamiseta
    {
        Task<ICollection<CamisetaDTO>> ListAsync();
        Task<CamisetaDTO> FindByIdAsync(int id);
        Task AddAsync(CamisetaDTO dto);
        Task UpdateAsync(int id, CamisetaDTO dto);
        Task DeleteAsync(int id);
        Task<ICollection<CamisetaDTO>> GetCamisetaByCategoria(int idCategoria);
        Task<ICollection<CamisetaDTO>> GetCamisetaByEquipo(short idEquipo);
        Task<ICollection<CamisetaDTO>> GetCamisetaByJugador(int idJugador);
        Task<ICollection<CamisetaDTO>> GetCamisetaByNombre(string nombre);
        Task<ICollection<CamisetaDTO>> GetCamisetasAutografiadas();
        Task<ICollection<CamisetaDTO>> GetCamisetasByVendedor(int idUsuarioVendedor);
        Task<ICollection<CamisetaDTO>> GetCamisetasByTemporada(short temporada);
        Task<ICollection<CamisetaDTO>> GetCamisetasVendidas();
        Task<ICollection<CamisetaDTO>> GetCamisetasEnSubasta();
        Task<ICollection<CamisetaDTO>> GetCamisetasSinSubasta();
    }
}
