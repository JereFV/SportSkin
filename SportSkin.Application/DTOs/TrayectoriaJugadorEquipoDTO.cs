using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record TrayectoriaJugadorEquipoDTO
    {
        [DisplayName("Jugador")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public int IdJugador { get; set; }

        [DisplayName("Equipo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, short.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public short IdEquipo { get; set; }

        [DisplayName("Fecha de Inicio")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public DateOnly FechaInicio { get; set; }

        [DisplayName("Fecha de Fin")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public DateOnly FechaFin { get; set; }

        // Navegaciones
        [DisplayName("Equipo")]
        public EquipoDTO IdEquipoNavigation { get; set; } = new();

        [DisplayName("Jugador")]
        public JugadorDTO IdJugadorNavigation { get; set; } = new();
    }
}
