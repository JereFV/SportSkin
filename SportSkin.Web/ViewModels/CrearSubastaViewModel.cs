using Microsoft.AspNetCore.Mvc.Rendering;
using SportSkin.Application.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    /*
     ViewModel para crear subastas.
     El usuario vendedor se asigna desde Session — nunca desde la interfaz.
    */
    public class CrearSubastaViewModel
    {
        public int IdUsuarioVendedor { get; set; }
        [DisplayName("Camiseta")]
        [Required(ErrorMessage = "Debe seleccionar una camiseta")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una camiseta válida")]
        public int IdCamiseta { get; set; }

        [DisplayName("Fecha de Inicio")]
        [Required(ErrorMessage = "La {0} es requerida")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de Cierre")]
        [Required(ErrorMessage = "La {0} es requerida")]
        public DateTime FechaCierre { get; set; }

        [DisplayName("Precio Base")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int PrecioBase { get; set; }

        [DisplayName("Incremento Mínimo")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int IncrementoMinimo { get; set; }

        [DisplayName("Precio de Compra Inmediata")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int PrecioCompraInmediata { get; set; }

        [Range(0, 100)]
        public double PorcentajeComision { get; set; } = 5.0;

        // Solo para mostrar en el modal — NO editable
        public string? NombreVendedor { get; set; }

        // Opciones del dropdown de camisetas disponibles (sin subasta activa)
        public List<CamisetaDTO> CamisetasDisponibles { get; set; } = new();
    }
}
