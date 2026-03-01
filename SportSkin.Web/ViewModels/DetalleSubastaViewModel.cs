using SportSkin.Application.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class DetalleSubastaViewModel
    {
        public SubastaDTO Subasta { get; set; } = new();

        public string? SituacionFirma { get; set; }

        [DisplayName("Puja Actual")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public int PujaActual { get; set; }

        [DisplayName("Cantidad Total de Pujas")]       
        public int CantidadTotalPujas { get; set; }
    }
}
