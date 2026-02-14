using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class ParametroSubasta
{
    public byte IdParametroSubasta { get; set; }

    public string Nombre { get; set; } = null!;

    public int Valor { get; set; }
}
