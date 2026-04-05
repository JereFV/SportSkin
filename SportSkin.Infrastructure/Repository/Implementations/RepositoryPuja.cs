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
    public class RepositoryPuja : IRepositoryPuja
    {
        private readonly SportSkinContext _context;

        public RepositoryPuja(SportSkinContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Puja entity)
        {
            await _context.Puja.AddAsync(entity);

            await _context.SaveChangesAsync();
        }
    }
}
