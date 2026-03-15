using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public class TrayectoriaJugadorAPIFootballDTO
    {
        [JsonPropertyName("team")]
        public EquipoAPIFootballDTO? Equipo { get; set; }

        [JsonPropertyName("seasons")]
        public int[]? Temporadas { get; set; }
    }

    public class ResponseTemporadasAPIFootball
    {
        [JsonPropertyName("response")]
        public ICollection<TrayectoriaJugadorAPIFootballDTO>? Equipos { get; set; }
    }  
}


