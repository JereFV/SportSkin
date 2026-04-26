using System;
using System.Collections.Generic;

namespace SportSkin.Infrastructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string? Apellido2 { get; set; }

    public byte IdRolUsuario { get; set; }

    public string Telefono { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string Usuario1 { get; set; } = null!;

    public string Contrasenna { get; set; } = null!;

    public byte? IdPreguntaRecuperacion { get; set; }

    public string? RespuestaPreguntaRecuperacion { get; set; }

    public virtual ICollection<Camiseta> Camiseta { get; set; } = new List<Camiseta>();

    public virtual PreguntaRecuperacionUsuario? IdPreguntaRecuperacionNavigation { get; set; }

    public virtual RolUsuario IdRolUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Puja> Puja { get; set; } = new List<Puja>();

    public virtual ICollection<Subasta> Subasta { get; set; } = new List<Subasta>();
}
