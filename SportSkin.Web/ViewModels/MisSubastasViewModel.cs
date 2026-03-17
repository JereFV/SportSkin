using SportSkin.Application.DTOs;

namespace SportSkin.Web.ViewModels
{
    public class MisSubastasViewModel
    {
        public IEnumerable<SubastaDTO> Subastas { get; set; } = new List<SubastaDTO>();
        public CrearSubastaViewModel FormCrear { get; set; } = new();
        public int IdVendedor { get; set; }
        public string NombreVendedor { get; set; } = string.Empty;
    }
}
