using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Camiseta
{
    public int IdCamiseta { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public byte IdCategoria { get; set; }

    public byte IdCondicionCamiseta { get; set; }

    public short IdEquipo { get; set; }

    public int IdJugador { get; set; }

    public short Temporada { get; set; }

    public bool Autografiada { get; set; }

    public int IdUsuarioVendedor { get; set; }

    public byte IdEstadoCamiseta { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaModificacion { get; set; }

    public bool EstadoRegistro { get; set; }

    public virtual CategoriaCamiseta IdCategoriaNavigation { get; set; } = null!;

    public virtual CondicionCamiseta CondicionCamisetaNavigation { get; set; } = null!;

    public virtual Equipo EquipoNavigation { get; set; } = null!;

    public virtual EstadoCamiseta EstadoCamisetaNavigation { get; set; } = null!;

    public virtual Jugador IdJugadorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioVendedorNavigation { get; set; } = null!;

    public virtual ICollection<ImagenCamiseta> ImagenCamiseta { get; set; } = new List<ImagenCamiseta>();

    public virtual ICollection<Subasta> Subasta { get; set; } = new List<Subasta>();
}
