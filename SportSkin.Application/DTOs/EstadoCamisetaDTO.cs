using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record EstadoCamisetaDTO
    {
        [DisplayName("Identificador Estado")]
        public byte IdEstadoCamiseta { get; set; }

        [DisplayName("Nombre Estado")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Nombre { get; set; } = string.Empty;
    }
}
