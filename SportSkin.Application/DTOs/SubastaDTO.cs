using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record SubastaDTO
    {
        [DisplayName("Identificador de Subasta")]
        public int IdSubasta { get; set; }

        [DisplayName("Fecha de Inicio")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de Cierre")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public DateTime FechaCierre { get; set; }

        [DisplayName("Fecha de Compra")]
        public DateTime? FechaCompra { get; set; }

        [DisplayName("Precio Base")]
        [DisplayFormat(DataFormatString = "${0:N0}")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int PrecioBase { get; set; }

        [DisplayName("Incremento Mínimo")]
        [DisplayFormat(DataFormatString = "${0:N0}")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int IncrementoMinimo { get; set; }

        [DisplayName("Precio Compra Inmediata")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser mayor a 0")]
        public int PrecioCompraInmediata { get; set; }

        [DisplayName("Monto de Compra")]
        public int? MontoCompra { get; set; }

        [DisplayName("Porcentaje Comisión")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(0, 100, ErrorMessage = "El {0} debe estar entre {1} y {2}")]
        public double PorcentajeComision { get; set; }

        [DisplayName("Monto Comisión")]
        public double? MontoComision { get; set; }

        [DisplayName("Camiseta")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una {0}")]
        public int IdCamiseta { get; set; }

        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public byte IdEstadoSubasta { get; set; }

        [DisplayName("Usuario Comprador")]
        public int? IdUsuarioComprador { get; set; }

        // Navegaciones
        [DisplayName("Camiseta")]
        public CamisetaDTO IdCamisetaNavigation { get; set; } = new();

        [DisplayName("Estado Actual")]
        public EstadoSubastaDTO IdEstadoSubastaNavigation { get; set; } = new();

        [DisplayName("Usuario Comprador")]
        public UsuarioDTO? IdUsuarioCompradorNavigation { get; set; }

        // Colecciones
        public List<DatosEnvioDTO> DatosEnvio { get; set; } = new();
        public List<FacturaDTO> Factura { get; set; } = new();
        public List<PujaDTO> Puja { get; set; } = new();
    }
}
