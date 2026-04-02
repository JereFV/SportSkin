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

        
        // Transiciones de estado manuales (para botones)
        Task PublicarAsync(int id);
        Task CancelarAsync(int id);
        
        // ── Transiciones automáticas (Background Service) ─────────────
        Task<int> ActivarSubastasPendientesAsync();
        Task<ICollection<Subasta>> CerrarSubastasVencidasAsync();


        //Valida cuando es el próximo evento para actualizar estados de subasta
        Task<DateTime?> GetProximoEventoAsync();
        

        // Validación de negocio
        Task<bool> CamisetaTieneSubastaActivaAsync(int idCamiseta, int? excluirIdSubasta = null);
        
        // Filtros
        Task<ICollection<Subasta>> GetSubastasActivasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<Subasta>> GetSubastasFinalizadasAsync(DateTime? desde, DateTime? hasta);
        Task<ICollection<Subasta>> GetSubastasVendidasAsync();
        Task<ICollection<Subasta>> GetSubastasByVendedorAsync(int idUsuarioVendedor);

        //Para el home
        Task<ICollection<Subasta>> GetSubastasMasPopularesAsync(int top);
    }
}
