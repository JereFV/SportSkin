using Microsoft.Extensions.Configuration;
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
    public class ServiceTrayectoriaJugador : IServiceTrayectoriaJugador
    {
        private readonly IConfiguration _configuration;

        public ServiceTrayectoriaJugador(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ICollection<int>> ListTemporadasJugadorByTeamAsync(int idEquipo, int idJugador)
        {
            //Variables definidas en configuración.
            var apiKey = _configuration["APIFootballSettings:APIKey"];
            var url = $"{_configuration["APIFootballSettings:SeasonsUrl"]}{idJugador}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            var respuestaAPI = await client.GetAsync(url);

            //Deserealizado de respuesta.
            ResponseTemporadasAPIFootball? trayectoriaJugador = JsonSerializer.Deserialize<ResponseTemporadasAPIFootball>(await respuestaAPI.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

            //Se accede a las propiedades de la respuesta para obtener las temporadas en el equipo específico.
            int[] temporadas = trayectoriaJugador?.Equipos?.Where(x => x.Equipo?.IdEquipo == idEquipo).FirstOrDefault()?.Temporadas ?? [];
                     
            return temporadas;
        }
    }
}
