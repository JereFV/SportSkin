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
    public class RepositoryCategoriaCamiseta : IRepositoryCategoriaCamiseta
    {
        private readonly SportSkinContext _context; 

        public RepositoryCategoriaCamiseta(SportSkinContext context)
        {
            _context = context;
        }

        public async Task<ICollection<CategoriaCamiseta>> ListAsync()
        {
            var categorias = await _context.Set<CategoriaCamiseta>()
                                     .AsNoTracking()
                                     .ToListAsync();

            return categorias;
        }
    }
}
