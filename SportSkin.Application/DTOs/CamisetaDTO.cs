using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record CamisetaDTO
    {
        [DisplayName("Identificador de la camiseta")]
        public int IdCamiseta { get; }

        [DisplayName("Nombre Camisa")]
        [Required(ErrorMessage ="{0} es un dato requerido")]
        public string Nombre {  get; set; } = string.Empty;

        [DisplayName("Descripción")]
        [Required(ErrorMessage ="{0} es un dato requerido")]
        public string Descripcion {  get; set; } = string.Empty;

        [DisplayName("Categoria")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public byte IdCategoria { get; set; }

        [DisplayName("Condición de la Camiseta")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public byte IdCondicionCamiseta { get; set; }

        [DisplayName("Equipo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public short IdEquipo {  get; set; }

        [DisplayName("Jugador")]
        [Required(ErrorMessage ="{0} es un dato requerido")]
        public int IdJugador {  get; set; }

        [DisplayName("Temporada")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public short Temporada { get; set; }

        [DisplayName("¿Autografiada?")]
        public bool Autografiada { get; set; }

        [DisplayName("Vendedor")]
        public int IdUsuarioVendedor { get; set; }

        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaModificacion { get; set; }

        public CategoriaCamisetaDTO IdCategoriaNavigation { get; set; } = new();
        public CondicionCamisetaDTO IdCondicionCamisetaNavigation { get; set; } = new();
        public EquipoDTO IdEquipoNavigation { get; set; } = new();
        public JugadorDTO IdJugadorNavigation { get; set; } = new();
        public UsuarioDTO IdUsuarioVendedorNavigation { get; set; } = new();

        public List<ImagenCamisetaDTO> ImagenCamiseta { get; set; } = new();
        public List<SubastaDTO> Subasta { get; set; } = new();

    }
}
