using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record MetodoPagoDTO
    {
        [DisplayName("Identificador Método de Pago")]
        public byte IdMetodoPago { get; set; }

        [DisplayName("Nombre Método de Pago")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        // Colección
        public List<FacturaDTO> Factura { get; set; } = new();
    }
}
