using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Interfaces
{
    public interface IRepositoryCondicionCamiseta
    {
        Task<ICollection<CondicionCamiseta>> ListAsync();
    }
}
