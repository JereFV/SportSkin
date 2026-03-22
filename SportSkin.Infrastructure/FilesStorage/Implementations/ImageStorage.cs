using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SportSkin.Infrastructure.FilesStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.FilesStorage.Implementations
{
    public class ImageStorage : IImageStorage
    {
        private readonly string _rutaBase;
        private readonly IConfiguration _configuration;

        public ImageStorage (IConfiguration configuration)
        {
            _configuration = configuration;
            _rutaBase = Path.Combine("wwwroot", _configuration["ImageSettings:StoragePath"] ?? "", "Camiseta");
        }

        public async Task<string> SaveImageAsync(int idCamiseta, IFormFile imagen)
        {
            string directorioIamgen = Path.Combine(_rutaBase, idCamiseta.ToString());
            string rutaImagen = Path.Combine(directorioIamgen, Path.GetFileName(imagen.FileName));

            //Crea el directorio de imágenes de camiseta si no existe previamente.
            if (!Directory.Exists(directorioIamgen))
                Directory.CreateDirectory(directorioIamgen);

            using (Stream stream = new FileStream(rutaImagen, FileMode.Create))
                await imagen.CopyToAsync(stream);

            //Devuelve la ruta de la imagen posterior al directorio wwwroot.
            return rutaImagen.Split("wwwroot")[1].Replace("\\", "/");
        }

        //Borra todas las imagenes contenidas en un directorio de camiseta específico.
        public void DeleteImages(int idCamiseta)
        {
            string rutaImagenes = _rutaBase + idCamiseta.ToString();

            if (Directory.Exists(rutaImagenes))
            {
                foreach (string archivo in Directory.GetFiles(rutaImagenes))
                {
                    File.Delete(archivo);
                }
            }         
        }
    }
}
