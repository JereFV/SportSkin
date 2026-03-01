using SportSkin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Interfaces
{
    public interface IServiceSubasta
    {
        Task<ICollection<SubastaDTO>> ListAsync();
        Task<SubastaDTO> FindByIdAsync(int id);
        Task<int> AddAsync(SubastaDTO dto);
        Task UpdateAsync(int id, SubastaDTO dto);
        Task DeleteAsync(int id);

        // Filtros
        Task<ICollection<SubastaDTO>> GetSubastasActivasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<SubastaDTO>> GetSubastasFinalizadasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<SubastaDTO>> GetSubastasVendidasAsync();
        Task<ICollection<SubastaDTO>> GetSubastasByVendedorAsync(int idUsuarioVendedor);

        //Para el home
        Task<ICollection<SubastaDTO>> GetSubastasMasPopularesAsync(int top);
    }
}
