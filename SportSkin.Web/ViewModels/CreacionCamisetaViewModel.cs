using SportSkin.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SportSkin.Web.ViewModels
{
    public class CreacionCamisetaViewModel
    {
        //DTO contenedor de los datos ingresados en el formulario.
        public CamisetaDTO CamisetaDTO { get; set; } = new();

        [Required(ErrorMessage = "Debe seleccionar al menos una categoría.")]
        public int[] CategoriasSeleccionadas { get; set; } = [];

        //Estructuras JSON auxiliares para transportar los datos seleccionados en los campos de Equipo y Jugador.        
        public string EquipoAPIFootballJSON { get; set; } = string.Empty;
        
        public string JugadorAPIFootballJSON { get; set; } = string.Empty;
       
        //Colección de imagenes adjuntas.
        public List<IFormFile> ImagenesCamiseta { get; set; } = [];

        //Catálogos y datos de visualización estáticos.
        public ICollection<CategoriaCamisetaDTO>? CategoriasCamiseta { get; set; }

        public ICollection<CondicionCamisetaDTO>? CondicionesCamiseta { get; set; }

        public string? NombreCompletoVendedor { get; set; }
    }
}
