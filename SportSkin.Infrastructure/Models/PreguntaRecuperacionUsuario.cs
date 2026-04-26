using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class PreguntaRecuperacionUsuario
{
    public byte IdPregunta { get; set; }

    public string Pregunta { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
