using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryCategoriaCamiseta
    {
        Task<ICollection<CategoriaCamiseta>> ListAsync();
    }
}
