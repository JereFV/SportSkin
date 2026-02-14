using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class ImagenCamiseta
{
    public byte IdImagen { get; set; }

    public int IdCamiseta { get; set; }

    public string RutaImagen { get; set; } = null!;

    public virtual Camiseta IdCamisetaNavigation { get; set; } = null!;
}
