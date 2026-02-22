using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record PaisDTO
    {
        [DisplayName("Identificador País")]
        public byte IdPais { get; set; }

        [DisplayName("Nombre País")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Zona de Envío")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public byte IdZonaEnvio { get; set; }

        // Navegación
        [DisplayName("Zona de Envío")]
        public ZonaEnvioDTO IdZonaEnvioNavigation { get; set; } = new();

        // Colección
        public List<DatosEnvioDTO> DatosEnvio { get; set; } = new();
    }
}
