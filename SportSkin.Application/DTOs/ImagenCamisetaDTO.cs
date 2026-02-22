using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record ImagenCamisetaDTO
    {
        [DisplayName("Identificador Imagen")]
        public byte IdImagen { get; set; }

        [DisplayName("Camiseta")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public int IdCamiseta { get; set; }

        [DisplayName("Ruta de Imagen")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(500, ErrorMessage = "{0} no puede superar {1} caracteres")]
        public string RutaImagen { get; set; } = string.Empty;

        // Navegación
        [DisplayName("Camiseta")]
        public CamisetaDTO IdCamisetaNavigation { get; set; } = new();
    }
}
