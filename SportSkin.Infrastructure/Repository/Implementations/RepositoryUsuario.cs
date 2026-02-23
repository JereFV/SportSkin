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
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly SportSkinContext _context;

        public RepositoryUsuario(SportSkinContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Usuario>> ListAsync()
        {
            var usuarios = await _context.Set<Usuario>()
                                         .AsNoTracking()
                                         .ToListAsync();

            return usuarios;
        }
    }
}
