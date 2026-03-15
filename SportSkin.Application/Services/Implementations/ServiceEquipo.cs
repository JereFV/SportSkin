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
    public class ServiceEquipo : IServiceEquipo
    {
        private readonly IConfiguration _configuration;

        public ServiceEquipo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ICollection<EquipoDTO>> ListEquiposFromAPIAsync(string filtro)
        {   
            //Variables definidas en configuración.
            var apiKey = _configuration["APIFootballSettings:APIKey"];
            var url = _configuration["APIFootballSettings:TeamsUrl"] + filtro;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            var respuestaAPI = await client.GetAsync(url);

            JObject equipos = JObject.Parse(await respuestaAPI.Content.ReadAsStringAsync());
            ICollection<EquipoDTO> equiposDTO = [];

            //Iteración de la estructura dinámica JObject.
            foreach (var equipo in equipos["response"] ?? Enumerable.Empty<JToken>())
            {
                JToken? detalleEquipo = equipo["team"];

                if (detalleEquipo != null)
                {
                    //Creación de entidades DTO para retorno a la interfaz.
                    EquipoDTO equipoDTO = new()
                    {
                        IdEquipo = (short)(detalleEquipo["id"] ?? 0),
                        Nombre = detalleEquipo["name"]?.ToString() ?? string.Empty,
                    };

                    equiposDTO.Add(equipoDTO);
                }
            }

            return equiposDTO;
        }
    }
}
