using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Pais
{
    public byte IdPais { get; set; }

    public string Nombre { get; set; } = null!;

    public byte IdZonaEnvio { get; set; }

    public virtual ICollection<DatosEnvio> DatosEnvio { get; set; } = new List<DatosEnvio>();

    public virtual ZonaEnvio IdZonaEnvioNavigation { get; set; } = null!;
}
