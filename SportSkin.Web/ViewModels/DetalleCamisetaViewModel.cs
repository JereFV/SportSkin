using SportSkin.Application.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class DetalleCamisetaViewModel
    {
        public CamisetaDTO Camiseta { get; set; } = new ();

        [DisplayName("Propietario")]
        public string? NombreCompletoPropietario { get; set; }

        [DisplayName("Fecha de Creación")]
        public string? FechaRegistroFormateada { get; set; }

        public string? SituacionFirma { get; set; }
    }
}
