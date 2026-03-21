using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.FilesStorage.Interfaces
{
    public interface IImageStorage
    {
        Task<string> SaveImageAsync(int idCamiseta, IFormFile image);
        void DeleteImages(int idCamiseta);
    }
}
