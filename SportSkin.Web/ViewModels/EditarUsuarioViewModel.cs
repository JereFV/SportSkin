using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class EditarUsuarioViewModel
    {
        public int IdUsuario { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Primer Apellido")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres")]
        public string Apellido1 { get; set; } = string.Empty;

        [DisplayName("Segundo Apellido")]
        [StringLength(100, ErrorMessage = "El {0} no puede superar {1} caracteres")]
        public string? Apellido2 { get; set; }

        [DisplayName("Correo Electrónico")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [EmailAddress(ErrorMessage = "El {0} no tiene un formato válido")]
        public string Correo { get; set; } = string.Empty;

        [DisplayName("Teléfono")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [Phone(ErrorMessage = "El {0} no tiene un formato válido")]
        public string Telefono { get; set; } = string.Empty;

        // Solo para mostrar, NO editable
        public string? Rol { get; set; }
        public DateTime FechaCreacion { get; set; }

        public bool EsPerfil { get; set; }
    }
}
