using SportSkin.Application.DTOs;

namespace SportSkin.Application.Services.Interfaces
{
    public interface IServicePago
    {
        Task<ICollection<FacturaDTO>> ListAsync();
        Task<string> RegistrarAsync(int idSubasta, byte idMetodoPago);
        Task ConfirmarAsync(string idFactura);
        Task<ICollection<SubastaDTO>> GetSubastasPendientesPagoAsync();
    }
}