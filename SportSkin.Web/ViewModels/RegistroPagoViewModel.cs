using SportSkin.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class RegistroPagoViewModel
    {
        // ── Datos de contexto (solo lectura en la vista) ──
        public int IdSubasta { get; set; }
        public string NombreCamiseta { get; set; } = string.Empty;
        public string NombreComprador { get; set; } = string.Empty;
        public double MontoTotal { get; set; }
        public double PorcentajeComision { get; set; }
        public double MontoComision { get; set; }
        public DateTime FechaCierre { get; set; }

        // Factura existente (si ya fue registrada)
        public FacturaDTO? FacturaExistente { get; set; }

        // ── Campos del formulario ──
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        [Display(Name = "Método de Pago")]
        public byte IdMetodoPago { get; set; }

        // Catálogo
        public ICollection<MetodoPagoDTO> MetodosPago { get; set; } = new List<MetodoPagoDTO>();
    }
}
