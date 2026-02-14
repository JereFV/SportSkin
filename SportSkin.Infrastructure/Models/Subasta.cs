using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Subasta
{
    public int IdSubasta { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaCierre { get; set; }

    public DateTime? FechaCompra { get; set; }

    public int PrecioBase { get; set; }

    public int IncrementoMinimo { get; set; }

    public int PrecioCompraInmediata { get; set; }

    public int? MontoCompra { get; set; }

    public double PorcentajeComision { get; set; }

    public double? MontoComision { get; set; }

    public int IdCamiseta { get; set; }

    public byte IdEstadoSubasta { get; set; }

    public int? IdUsuarioComprador { get; set; }

    public virtual ICollection<DatosEnvio> DatosEnvio { get; set; } = new List<DatosEnvio>();

    public virtual ICollection<Factura> Factura { get; set; } = new List<Factura>();

    public virtual Camiseta IdCamisetaNavigation { get; set; } = null!;

    public virtual EstadoSubasta IdEstadoSubastaNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioCompradorNavigation { get; set; }

    public virtual ICollection<Puja> Puja { get; set; } = new List<Puja>();
}
