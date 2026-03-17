using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class EditarSubastaViewModel
    {
        public int IdSubasta { get; set; }

        [DisplayName("Camiseta")]
        [Required(ErrorMessage = "Debe seleccionar una camiseta")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una camiseta válida")]
        public int IdCamiseta { get; set; }

        [DisplayName("Fecha de Inicio")]
        [Required(ErrorMessage = "{0} es requerida")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de Cierre")]
        [Required(ErrorMessage = "{0} es requerida")]
        public DateTime FechaCierre { get; set; }

        [DisplayName("Precio Base")]
        [Required(ErrorMessage = "{0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int PrecioBase { get; set; }

        [DisplayName("Incremento Mínimo")]
        [Required(ErrorMessage = "{0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int IncrementoMinimo { get; set; }

        [DisplayName("Precio Compra Inmediata")]
        [Required(ErrorMessage = "{0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int PrecioCompraInmediata { get; set; }

        [DisplayName("Porcentaje Comisión")]
        [Required(ErrorMessage = "{0} es requerido")]
        [Range(0, 100, ErrorMessage = "El {0} debe estar entre {1} y {2}")]
        public double PorcentajeComision { get; set; }

        // Informativos (no editables)
        public string? NombreCamiseta { get; set; }
        public string? NombreVendedor { get; set; }
        public string? EstadoActual { get; set; }
    }
}
