using SportSkin.Application.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class InterfazSubastaViewModel
    {
        public SubastaDTO Subasta { get; set; } = new();

        public string? SituacionFirma { get; set; }
      
        [DisplayFormat(DataFormatString = "${0:N0}")]
        public int PujaActual { get; set; }
        
        public int CantidadTotalPujas { get; set; }

        public string? NombreCompletoVendedor { get; set; }

        public string? InicialesVendedor { get; set; }

        public string? NombreCompletoJugador { get; set; }

        //Datos utilizados por el modal de Pujar.
        [DisplayFormat(DataFormatString = "${0:N0}")]
        public int MontoMinProximaPuja { get; set; }
    }
}
