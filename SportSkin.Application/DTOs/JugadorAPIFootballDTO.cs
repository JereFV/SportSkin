using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public class JugadorAPIFootballDTO
    {
        [JsonPropertyName("id")]
        public int IdJugador { get; set; }

        [JsonPropertyName("name")]
        public string? Nombre { get; set; }
    }

    public class ResponseJugadoresAPIFootball
    {
        [JsonPropertyName("response")]
        public ICollection<JugadorWrapper>? Jugadores { get; set; }
    }

    public class JugadorWrapper
    {
        [JsonPropertyName("player")]
        public JugadorAPIFootballDTO DatosJugador { get; set; } = new ();
    }
}
