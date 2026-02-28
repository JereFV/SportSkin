using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class CategoriaCamiseta
{
    public byte IdCategoriaCamiseta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Camiseta> IdCamiseta { get; set; } = new List<Camiseta>();
}
