using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class MetodoPago
{
    public byte IdMetodoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Factura> Factura { get; set; } = new List<Factura>();
}
