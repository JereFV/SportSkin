using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositoryPreguntaRecuperacionUsuario : IRepositoryPreguntaRecuperacionUsuario
    {
        private readonly SportSkinContext _context;

        public RepositoryPreguntaRecuperacionUsuario(SportSkinContext context) 
        {
            _context = context;
        } 

        public async Task<ICollection<PreguntaRecuperacionUsuario>> ListAsync()
        {
            return await _context.Set<PreguntaRecuperacionUsuario>()
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }
}
