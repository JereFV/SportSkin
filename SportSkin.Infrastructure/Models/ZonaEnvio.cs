using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class ZonaEnvio
{
    public byte IdZonaEnvio { get; set; }

    public string Nombre { get; set; } = null!;

    public int Tarifa { get; set; }

    public virtual ICollection<Pais> Pais { get; set; } = new List<Pais>();
}
