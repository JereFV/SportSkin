using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class TrayectoriaJugadorEquipo
{
    public int IdJugador { get; set; }

    public short IdEquipo { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public virtual Equipo IdEquipoNavigation { get; set; } = null!;

    public virtual Jugador IdJugadorNavigation { get; set; } = null!;
}
