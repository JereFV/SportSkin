using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record EquipoDTO
    {
        [DisplayName("Identificador Equipo")]
        public short IdEquipo { get; set; }

        [DisplayName("Nombre Equipo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("ID Externo Equipo")]
        public short IdExternoEquipo { get; set; }

        [DisplayName("País")]
        public string? Pais { get; set; }

        [DisplayName("¿Es Selección Nacional?")]
        public bool EsSeleccionNacional { get; set; }

        // Colecciones
        public List<CamisetaDTO> Camiseta { get; set; } = new();
        public List<TrayectoriaJugadorEquipoDTO> TrayectoriaJugadorEquipo { get; set; } = new();
    }
}
