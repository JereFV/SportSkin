using System;
using System.Collections.Generic;

namespace SportSkin.Web.ViewModels
{
    public class PujaHistorialVM
    {
        public int IdPuja { get; set; }
        public string NombreCamiseta { get; set; }
        public string ImagenCamiseta { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPuja { get; set; }
        public string EstadoSubasta { get; set; }
        public bool EsGanador { get; set; }
    }

    public class SubastaHistorialVM
    {
        public int IdSubasta { get; set; }
        public string NombreCamiseta { get; set; }
        public string ImagenCamiseta { get; set; }
        public decimal PrecioInicial { get; set; }
        public decimal? PrecioFinal { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }
        public int TotalPujas { get; set; }
    }

    public class CompraHistorialVM
    {
        public string IdPago { get; set; }
        public string NombreCamiseta { get; set; }
        public string ImagenCamiseta { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaCompra { get; set; }
        public string EstadoPago { get; set; }
        public string NombreVendedor { get; set; }
    }

    public class VentaHistorialVM
    {
        public string IdPago { get; set; }
        public string NombreCamiseta { get; set; }
        public string ImagenCamiseta { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaVenta { get; set; }
        public string EstadoPago { get; set; }
        public string NombreComprador { get; set; }
    }

    public class PagoHistorialVM
    {
        public string IdPago { get; set; }
        public string NombreCamiseta { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
        public string NumeroReferencia { get; set; }
    }

    public class MiHistorialVM
    {
        public string Rol { get; set; }

        // Comprador
        public List<PujaHistorialVM> Pujas { get; set; } = new();
        public List<CompraHistorialVM> Compras { get; set; } = new();
        public List<PagoHistorialVM> Pagos { get; set; } = new();

        // Vendedor
        public List<SubastaHistorialVM> Subastas { get; set; } = new();
        public List<VentaHistorialVM> Ventas { get; set; } = new();
    }
}
