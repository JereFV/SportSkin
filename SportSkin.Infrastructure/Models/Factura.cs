using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Factura
{
    public string IdFactura { get; set; } = null!;

    public int? IdSubasta { get; set; }

    public double Total { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaPago { get; set; }

    public byte IdEstadoFactura { get; set; }

    public byte IdMetodoPago { get; set; }

    public virtual EstadoFactura IdEstadoFacturaNavigation { get; set; } = null!;

    public virtual MetodoPago IdMetodoPagoNavigation { get; set; } = null!;

    public virtual Subasta? IdSubastaNavigation { get; set; }
}
