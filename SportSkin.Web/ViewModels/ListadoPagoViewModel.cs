using SportSkin.Application.DTOs;

namespace SportSkin.Web.ViewModels
{
    public class ListadoPagoViewModel
    {
        // Fila del listado
        public string IdFactura { get; set; } = string.Empty;
        public int IdSubasta { get; set; }
        public string NombreCamiseta { get; set; } = string.Empty;
        public string RutaImagenCamiseta { get; set; } = string.Empty;
        public string NombreComprador { get; set; } = string.Empty;
        public double Total { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaPago { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
        public byte IdEstadoFactura { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
    }

    public class PagoIndexViewModel
    {
        public ICollection<ListadoPagoViewModel> Pagos { get; set; } = new List<ListadoPagoViewModel>();
        public ICollection<MetodoPagoDTO> MetodosPago { get; set; } = new List<MetodoPagoDTO>();

        // Para el modal de registrar pago (subastas sin factura aún)
        public ICollection<SubastaPendientePagoViewModel> SubastasPendientes { get; set; } = new List<SubastaPendientePagoViewModel>();
    }

    public class SubastaPendientePagoViewModel
    {
        public int IdSubasta { get; set; }
        public string NombreCamiseta { get; set; } = string.Empty;
        public string RutaImagen { get; set; } = string.Empty;
        public string NombreComprador { get; set; } = string.Empty;
        public double MontoTotal { get; set; }
        public DateTime FechaCierre { get; set; }
    }
}