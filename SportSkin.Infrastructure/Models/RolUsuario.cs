using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class RolUsuario
{
    public byte IdRolUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
