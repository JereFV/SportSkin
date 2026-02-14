using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Jugador
{
    public int IdJugador { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public int IdExternoJugador { get; set; }

    public string Nacionalidad { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public virtual ICollection<Camiseta> Camiseta { get; set; } = new List<Camiseta>();

    public virtual ICollection<TrayectoriaJugadorEquipo> TrayectoriaJugadorEquipo { get; set; } = new List<TrayectoriaJugadorEquipo>();
}
