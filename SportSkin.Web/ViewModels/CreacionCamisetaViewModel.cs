using SportSkin.Application.DTOs;

namespace SportSkin.Web.ViewModels
{
    public class CreacionCamisetaViewModel
    {
        public ICollection<CategoriaCamisetaDTO>? CategoriasCamiseta { get; set; }
        public ICollection<CondicionCamisetaDTO>? CondicionesCamiseta { get; set; }
    }
}
