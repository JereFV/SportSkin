using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record ParametroSubastaDTO
    {
        [DisplayName("Identificador Parámetro")]
        public byte IdParametroSubasta { get; set; }

        [DisplayName("Nombre Parámetro")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Valor")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El {0} no puede ser negativo")]
        public int Valor { get; set; }
    }
}
