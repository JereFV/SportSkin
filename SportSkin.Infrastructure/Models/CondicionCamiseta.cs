using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class CondicionCamiseta
{
    public byte IdCondicionCamiseta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Camiseta> Camiseta { get; set; } = new List<Camiseta>();
}
