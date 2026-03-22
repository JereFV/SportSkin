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
    public class RepositoryEquipo : IRepositoryEquipo
    {
        private readonly SportSkinContext _context;

        public RepositoryEquipo(SportSkinContext context)
        {
            _context = context;
        }

        //Agrega un equipo a la entidad respectiva en caso de que no exista previamente.
        public async Task<Equipo> AddAsync(Equipo entity)
        {
            var equipo = await FindByIdExternoAsync(entity.IdExternoEquipo);

            //Si el equipo no existe, procede a ejecutar el Add y retornar la entidad enviada.
            if (equipo == null)
            {
                await _context.AddAsync(entity);

                equipo = entity;
            }

            return equipo;
        }

        public async Task<Equipo?> FindByIdExternoAsync(short idExterno)
        {
            var equipo = await _context.Set<Equipo>()
                                 .AsNoTracking()
                                 .Where(x => x.IdExternoEquipo == idExterno)
                                 .FirstOrDefaultAsync();

            return equipo;
        }
    }
}
