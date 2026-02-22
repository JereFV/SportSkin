using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record CategoriaCamisetaDTO
    {
        [DisplayName("Identificador Categoría")]
        public byte IdCategoriaCamiseta { get; set; }

        [DisplayName("Nombre Categoría")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        // Colección de camisetas en esta categoría
        public List<CamisetaDTO> Camiseta { get; set; } = new();
    }
}
