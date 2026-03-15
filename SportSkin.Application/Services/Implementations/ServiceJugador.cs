using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<ICollection<JugadorDTO>> ListJugadoresFromAPIAsync(string filtro, int idEquipo)
        {
            //Variables definidas en configuración.
            var apiKey = _configuration["APIFootballSettings:APIKey"];
            var url = $"{_configuration["APIFootballSettings:PlayersUrl"]}{filtro}&team={idEquipo}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            var respuestaAPI = await client.GetAsync(url);

            JObject jugadores = JObject.Parse(await respuestaAPI.Content.ReadAsStringAsync());
            ICollection<JugadorDTO> equiposDTO = [];

            //Iteración de la estructura dinámica JObject.
            foreach (var jugador in jugadores["response"] ?? Enumerable.Empty<JToken>())
            {
                JToken? detalleJugador = jugador["player"];

                if (detalleJugador != null)
                {
                    //Creación de entidades DTO para retorno a la interfaz.
                    JugadorDTO jugadorDTO = new()
                    {
                        IdJugador = (short)(detalleJugador["id"] ?? 0),
                        Nombre = detalleJugador["name"]?.ToString() ?? string.Empty,
                    };

                    equiposDTO.Add(jugadorDTO);
                }
            }

            return equiposDTO;
        }
    }
}
