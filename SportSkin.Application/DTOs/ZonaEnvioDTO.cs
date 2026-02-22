using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record ZonaEnvioDTO
    {
        [DisplayName("Identificador Zona de Envío")]
        public byte IdZonaEnvio { get; set; }

        [DisplayName("Nombre Zona")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Tarifa")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El {0} no puede ser negativo")]
        public int Tarifa { get; set; }

        // Colección
        public List<PaisDTO> Pais { get; set; } = new();
    }
}
