using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class CrearUsuarioViewModel
    {
        [DisplayName("Nombre")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [StringLength(100, MinimumLength = 2,
           ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Primer Apellido")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres")]
        public string Apellido1 { get; set; } = string.Empty;

        [DisplayName("Segundo Apellido")]
        [StringLength(100, ErrorMessage = "El {0} no puede superar {1} caracteres")]
        public string? Apellido2 { get; set; }

        [DisplayName("Rol")]
        [Required(ErrorMessage = "Debe seleccionar un {0}")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar un {0} válido")]
        public byte IdRolUsuario { get; set; }

        [DisplayName("Teléfono")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [Phone(ErrorMessage = "El {0} no tiene un formato válido")]
        public string Telefono { get; set; } = string.Empty;

        [DisplayName("Correo Electrónico")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [EmailAddress(ErrorMessage = "El {0} no tiene un formato válido")]
        public string Correo { get; set; } = string.Empty;

        [DisplayName("Nombre de Usuario")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [StringLength(50, MinimumLength = 4,
            ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres")]
        public string Usuario1 { get; set; } = string.Empty;

        [DisplayName("Contraseña")]
        [Required(ErrorMessage = "La {0} es requerida")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "La {0} debe tener al menos {2} caracteres")]
        [DataType(DataType.Password)]
        public string Contrasenna { get; set; } = string.Empty;

        [DisplayName("Confirmar Contraseña")]
        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Contrasenna", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasenna { get; set; } = string.Empty;

        // Opciones del dropdown de roles (se carga desde la BD)
        public List<SelectListItem> Roles { get; set; } = new();

    }
}
