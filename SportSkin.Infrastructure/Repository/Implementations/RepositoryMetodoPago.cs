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
    public class RepositoryMetodoPago : IRepositoryMetodoPago
    {
        private readonly SportSkinContext _context;
        public RepositoryMetodoPago(SportSkinContext context) => _context = context;

        public async Task<ICollection<MetodoPago>> ListAsync()
        {
            return await _context.Set<MetodoPago>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
