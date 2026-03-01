using SportSkin.Application.DTOs;

namespace SportSkin.Web.ViewModels
{
    public class HomeViewModel
    {
        public ICollection<SubastaDTO> SubastasMasPopulares { get; set; } = new List<SubastaDTO>();
        public int TotalSubastasActivas { get; set; }
        public int TotalCamisetas { get; set; }
    }
}
