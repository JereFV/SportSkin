using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record RolUsuarioDTO
    {
        [DisplayName("Identificador Rol")]
        public byte IdRolUsuario { get; set; }

        [DisplayName("Nombre Rol")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        // Colección de usuarios con este rol
        public List<UsuarioDTO> Usuario { get; set; } = new();
    }
}
