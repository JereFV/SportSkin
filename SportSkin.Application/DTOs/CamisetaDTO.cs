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
        [Required(ErrorMessage ="{0) es un dato requerido")]
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
    }
}
