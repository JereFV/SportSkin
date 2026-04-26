using SportSkin.Application.DTOs;
using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Interfaces
{
    public interface IServicePreguntaRecuperacionUsuario
    {
        Task<ICollection<PreguntaRecuperacionUsuarioDTO>> ListAsync();
    }
}
