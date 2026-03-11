using SportSkin.Application.DTOs;

namespace SportSkin.Web.ViewModels
{
    public class CamisetaViewModel
    {
        public ICollection<CamisetaDTO>? Camisetas { get; set; }

        public CreacionCamisetaViewModel CreacionCamiseta { get; set; } = new();
    }
}
