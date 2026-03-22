using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositoryJugador : IRepositoryJugador
    {
        private readonly SportSkinContext _context;

        public RepositoryJugador(SportSkinContext context)
        {
            _context = context;
        }

        //Agrega un jugador a la entidad respectiva en caso de que no exista previamente.
        public async Task<Jugador> AddAsync(Jugador entity)
        {
            var jugador = await FindByIdExternoAsync(entity.IdExternoJugador);

            //Si el jugador no existe, procede a ejecutar el Add y retornar la entidad enviada.
            if (jugador == null)
            {
                await _context.AddAsync(entity);

                jugador = entity;
            }

            return jugador;
        }

        public async Task<Jugador?> FindByIdExternoAsync(int idExterno)
        {
            var jugador = await _context.Set<Jugador>()
                                 .AsNoTracking()
                                 .Where(x => x.IdExternoJugador == idExterno)
                                 .FirstOrDefaultAsync();

            return jugador;
        }
    }
}
