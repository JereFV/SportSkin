using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class EstadoSubasta
{
    public byte IdEstadoSubasta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Subasta> Subasta { get; set; } = new List<Subasta>();
}
