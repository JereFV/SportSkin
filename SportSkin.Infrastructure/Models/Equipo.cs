using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Equipo
{
    public short IdEquipo { get; set; }

    public string Nombre { get; set; } = null!;

    public short IdExternoEquipo { get; set; }

    public string? Pais { get; set; }

    public bool EsSeleccionNacional { get; set; }

    public virtual ICollection<Camiseta> Camiseta { get; set; } = new List<Camiseta>();

    public virtual ICollection<TrayectoriaJugadorEquipo> TrayectoriaJugadorEquipo { get; set; } = new List<TrayectoriaJugadorEquipo>();
}
