using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record DatosEnvioDTO
    {
        [DisplayName("Identificador Datos de Envío")]
        public int IdDatosEnvio { get; set; }

        [DisplayName("Subasta")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public int IdSubasta { get; set; }

        [DisplayName("País")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public byte IdPais { get; set; }

        [DisplayName("Región")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Region { get; set; } = string.Empty;

        [DisplayName("Ciudad")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Ciudad { get; set; } = string.Empty;

        [DisplayName("Dirección Exacta")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string DireccionExacta { get; set; } = string.Empty;

        [DisplayName("Código Postal")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1000, 999999, ErrorMessage = "El {0} debe estar entre {1} y {2}")]
        public int CodigoPostal { get; set; }

        // Navegaciones
        [DisplayName("País")]
        public PaisDTO IdPaisNavigation { get; set; } = new();

        [DisplayName("Subasta")]
        public SubastaDTO IdSubastaNavigation { get; set; } = new();
    }
}
