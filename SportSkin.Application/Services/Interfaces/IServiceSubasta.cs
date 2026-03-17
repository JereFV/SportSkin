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


        // --- Transiciones manuales (botones en la UI) ---
        
        Task PublicarAsync(int id);
        Task CancelarAsync(int id);


        // --- Transiciones automáticas (Background Service) ---
        /*
        Task<int> ActivarSubastasPendientesAsync();
        Task<int> CerrarSubastasVencidasAsync();
        */


        Task<bool> PuedeEditarAsync(int id);
        Task<bool> PuedeCancelarAsync(int id);
        Task<bool> CamisetaTieneSubastaActivaAsync(int idCamiseta, int? excludeIdSubasta = null);


        // Filtros
        Task<ICollection<SubastaDTO>> GetSubastasActivasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<SubastaDTO>> GetSubastasFinalizadasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<SubastaDTO>> GetSubastasVendidasAsync();
        Task<ICollection<SubastaDTO>> GetSubastasByVendedorAsync(int idUsuarioVendedor);

        //Para el home
        Task<ICollection<SubastaDTO>> GetSubastasMasPopularesAsync(int top);
    }
}
