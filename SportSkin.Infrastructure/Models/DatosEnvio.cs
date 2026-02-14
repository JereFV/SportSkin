using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class DatosEnvio
{
    public int IdDatosEnvio { get; set; }

    public int IdSubasta { get; set; }

    public byte IdPais { get; set; }

    public string Region { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string DireccionExacta { get; set; } = null!;

    public int CodigoPostal { get; set; }

    public virtual Pais IdPaisNavigation { get; set; } = null!;

    public virtual Subasta IdSubastaNavigation { get; set; } = null!;
}
