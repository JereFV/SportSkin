using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record PreguntaRecuperacionUsuarioDTO
    {
        public byte IdPregunta { get; set; }

        public string Pregunta { get; set; } = null!;

        public List<UsuarioDTO> Usuario { get; set; } = [];
    }
}
