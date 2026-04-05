using SportSkin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Interfaces
{
    public interface IServicePuja
    {
        Task AddAsync(PujaDTO dto);
    }
}
