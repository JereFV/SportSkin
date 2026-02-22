using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record JugadorDTO
    {
        [DisplayName("Identificador Jugador")]
        public int IdJugador { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Apellido")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Apellido { get; set; } = string.Empty;

        [DisplayName("ID Externo Jugador")]
        public int IdExternoJugador { get; set; }

        [DisplayName("Nacionalidad")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Nacionalidad { get; set; } = string.Empty;

        [DisplayName("Fecha de Nacimiento")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public DateOnly FechaNacimiento { get; set; }

        // Colecciones
        public List<CamisetaDTO> Camiseta { get; set; } = new();
        public List<TrayectoriaJugadorEquipoDTO> TrayectoriaJugadorEquipo { get; set; } = new();
    }
}
