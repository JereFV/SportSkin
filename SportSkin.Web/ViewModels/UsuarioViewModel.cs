using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class UsuarioViewModel
    {
        [Display(Name = "Nombre Completo")]
        public string? NombreCompleto { get; set; }

        public string? Rol { get; set; }

        public string? Estado { get; set; }
    }
}
