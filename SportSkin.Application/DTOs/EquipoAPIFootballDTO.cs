using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public class EquipoAPIFootballDTO
    {
        [JsonPropertyName("id")]
        public int IdEquipo { get; set; }

        [JsonPropertyName("name")]
        public string? Nombre { get; set; }

        [JsonPropertyName("country")]
        public string? Pais { get; set; }

        [JsonPropertyName("national")]
        public bool EsNacional { get; set; }
    }

    public class ResponseEquiposAPIFootball
    {
        [JsonPropertyName("response")]
        public ICollection<EquipoWrapper>? Equipos { get; set; }
    }

    public class EquipoWrapper
    {
        [JsonPropertyName("team")]
        public EquipoAPIFootballDTO DatosEquipo { get; set; } = new ();
    }
}
