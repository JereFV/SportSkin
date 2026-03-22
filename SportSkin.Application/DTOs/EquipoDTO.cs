using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        [JsonPropertyName("name")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("ID Externo Equipo")]
        [JsonPropertyName("id")]
        public short IdExternoEquipo { get; set; }

        [DisplayName("País")]
        [JsonPropertyName("country")]
        public string? Pais { get; set; }

        [DisplayName("¿Es Selección Nacional?")]
        [JsonPropertyName("national")]
        public bool EsSeleccionNacional { get; set; }

        // Colecciones
        [JsonIgnore]
        public List<CamisetaDTO> Camiseta { get; set; } = new();
        
        public List<TrayectoriaJugadorEquipoDTO> TrayectoriaJugadorEquipo { get; set; } = new();
    }
}
