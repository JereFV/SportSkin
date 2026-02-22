using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record FacturaDTO
    {
        [DisplayName("Identificador Factura")]
        public string IdFactura { get; set; } = string.Empty;

        [DisplayName("Subasta")]
        public int? IdSubasta { get; set; }

        [DisplayName("Total")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public double Total { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Fecha de Pago")]
        public DateTime? FechaPago { get; set; }

        [DisplayName("Estado Factura")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public byte IdEstadoFactura { get; set; }

        [DisplayName("Método de Pago")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public byte IdMetodoPago { get; set; }

        // Navegaciones
        [DisplayName("Estado Factura")]
        public EstadoFacturaDTO IdEstadoFacturaNavigation { get; set; } = new();

        [DisplayName("Método de Pago")]
        public MetodoPagoDTO IdMetodoPagoNavigation { get; set; } = new();

        [DisplayName("Subasta")]
        public SubastaDTO? IdSubastaNavigation { get; set; }
    }
}
