using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record PujaDTO
    {
        [DisplayName("Identificador Puja")]
        public short IdPuja { get; set; }

        [DisplayName("Subasta")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public int IdSubasta { get; set; }

        [DisplayName("Monto")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int Monto { get; set; }

        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }

        [DisplayName("Usuario")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public int IdUsuarioPuja { get; set; }

        // Navegaciones
        [DisplayName("Subasta")]
        public SubastaDTO IdSubastaNavigation { get; set; } = new();

        [DisplayName("Usuario")]
        public UsuarioDTO IdUsuarioPujaNavigation { get; set; } = new();
    }
}
