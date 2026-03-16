using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class CambiarContrasennaViewModel
    {
        public int IdUsuario { get; set; }

        // Solo para mostrar en el modal, no se procesa
        public string? NombreCompleto { get; set; }

        [DisplayName("Nueva Contraseña")]
        [Required(ErrorMessage = "La {0} es requerida")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "La {0} debe tener al menos {2} caracteres")]
        [DataType(DataType.Password)]
        public string NuevaContrasenna { get; set; } = string.Empty;

        [DisplayName("Confirmar Contraseña")]
        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("NuevaContrasenna", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasenna { get; set; } = string.Empty;
    }
}
