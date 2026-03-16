using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations
{
    public class ServiceJugador : IServiceJugador
    {
        private readonly IConfiguration _configuration;

        public ServiceJugador(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ICollection<JugadorAPIFootballDTO>> ListJugadoresFromAPIAsync(string filtro, int idEquipo)
        {
            //Variables definidas en configuración.
            var apiKey = _configuration["APIFootballSettings:APIKey"];
            var url = $"{_configuration["APIFootballSettings:PlayersUrl"]}{filtro}&team={idEquipo}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            var respuestaAPI = await client.GetAsync(url);

            //Deserealizado de respuesta.
            ResponseJugadoresAPIFootball? responseJugadores = JsonSerializer.Deserialize<ResponseJugadoresAPIFootball>(await respuestaAPI.Content.ReadAsStringAsync());

            //Se accede a las propiedades de la respuesta para obtener los equipos coincidentes con el filtro.
            ICollection<JugadorAPIFootballDTO> jugadores = responseJugadores?.Jugadores?.Select(x => x.DatosJugador)?.ToList() ?? [];

            return jugadores;
        }
    }
}
