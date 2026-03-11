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
    public class RepositoryCondicionCamiseta : IRepositoryCondicionCamiseta
    {
        private readonly SportSkinContext _context;

        public RepositoryCondicionCamiseta(SportSkinContext context)
        {
            _context = context;
        }

        public async Task<ICollection<CondicionCamiseta>> ListAsync()
        {
            var condiciones = await _context.Set<CondicionCamiseta>()
                                     .AsNoTracking()
                                     .ToListAsync();

            return condiciones;
        }
    }
}
