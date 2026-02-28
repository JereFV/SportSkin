using SportSkin.Application.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class DetalleCamisetaViewModel
    {
        public CamisetaDTO? Camiseta { get; set; }

        [DisplayName("Propietario")]
        public string? NombreCompleto { get; set; }       
    }
}
