using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Puja
{
    public short IdPuja { get; set; }

    public int IdSubasta { get; set; }

    public int Monto { get; set; }

    public DateTime Fecha { get; set; }

    public int IdUsuarioPuja { get; set; }

    public virtual Subasta IdSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioPujaNavigation { get; set; } = null!;
}
