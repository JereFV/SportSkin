using Microsoft.AspNetCore.Http;
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
        Task<ICollection<CamisetaDTO>> ListAsyncByUser(int idUsuario);
        Task<CamisetaDTO> FindByIdAsync(int id);
        Task AddAsync(CamisetaDTO dto, ICollection<IFormFile> imagenes);
        Task UpdateAsync(CamisetaDTO dto, ICollection<IFormFile> imagenes, List<int> idsImagenesEliminadas);
        Task DeleteAsync(int id);
        Task<ICollection<CamisetaDTO>> GetCamisetaByCategoria(int idCategoria);
        Task<ICollection<CamisetaDTO>> GetCamisetaByEquipo(short idEquipo);
        Task<ICollection<CamisetaDTO>> GetCamisetaByJugador(int idJugador);
        Task<ICollection<CamisetaDTO>> GetCamisetaByNombre(string nombre);
        Task<ICollection<CamisetaDTO>> GetCamisetasAutografiadas();
        Task<ICollection<CamisetaDTO>> GetCamisetasByVendedor(int idUsuarioVendedor);
        Task<ICollection<CamisetaDTO>> GetCamisetasByTemporada(short temporada);
        Task<ICollection<CamisetaDTO>> GetCamisetasVendidas(int idUsuario);
        Task<ICollection<CamisetaDTO>> GetCamisetasEnSubasta(int idUsuario);
        Task<ICollection<CamisetaDTO>> GetCamisetasSinSubasta(int idUsuario);
        Task ChangeStateAsync(int id);
    }
}
