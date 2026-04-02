using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public async Task<ICollection<EquipoAPIFootballDTO>> ListEquiposFromAPIAsync(string filtro)
        {   
            //Variables definidas en configuración.
            var apiKey = _configuration["APIFootballSettings:APIKey"];
            var url = _configuration["APIFootballSettings:TeamsUrl"] + filtro;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            var respuestaAPI = await client.GetAsync(url);

            //Deserealizado de respuesta.
            ResponseEquiposAPIFootball? responseEquipos = JsonSerializer.Deserialize<ResponseEquiposAPIFootball>(await respuestaAPI.Content.ReadAsStringAsync());

            //Se accede a las propiedades de la respuesta para obtener los equipos coincidentes con el filtro.
            ICollection<EquipoAPIFootballDTO> equipos = responseEquipos?.Equipos?.Select(x => x.DatosEquipo)?.ToList() ?? [];

            //ICollection<EquipoAPIFootballDTO> equipos = new List<EquipoAPIFootballDTO>();

            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "Pumas", IdEquipo = 1, Pais = "Mexico", EsNacional = false });
            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "Pumas UNAM W", IdEquipo = 5, Pais = "Mexico", EsNacional = false });
            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "Pumas Tabasco", IdEquipo = 2, Pais = "Mexico", EsNacional = false });
            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "Pumas UNAM U20", IdEquipo = 3, Pais = "Mexico", EsNacional = false });
            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "Kitsap Pumas", IdEquipo = 4, Pais = "Mexico", EsNacional = false });
            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "UNAM Pumas U21", IdEquipo = 6, Pais = "Mexico", EsNacional = false });
            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "U.N.A.M. - Pumas", IdEquipo = 2286, Pais = "Mexico", EsNacional = false });
            //equipos.Add(new EquipoAPIFootballDTO { Nombre = "Pumas U19", IdEquipo = 8, Pais = "Mexico", EsNacional = false });

            return equipos;
        }
    }
}
