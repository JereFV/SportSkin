using Microsoft.AspNetCore.Http;
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
        public int IdCamiseta { get; set; }

        [DisplayName("Nombre Camisa")]
        [Required(ErrorMessage ="{0} es un dato requerido")]
        public string Nombre {  get; set; } = string.Empty;

        [DisplayName("Descripción")]
        [Required(ErrorMessage ="{0} es un dato requerido")]
        public string Descripcion {  get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} es un dato requerido")]
        public byte IdCondicionCamiseta { get; set; }

        [Required(ErrorMessage = "{0} es un dato requerido")]
        public short IdEquipo {  get; set; }

        [Required(ErrorMessage ="{0} es un dato requerido")]
        public int IdJugador {  get; set; }

        [DisplayName("Temporada")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public short Temporada { get; set; }

        [DisplayName("¿Autografiada?")]
        public bool Autografiada { get; set; }

        public int IdUsuarioVendedor { get; set; }

        public byte IdEstadoCamiseta { get; set; }

        public bool EstadoRegistro { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime FechaRegistro { get; set; }

        public DateTime FechaModificacion { get; set; }

        [DisplayName("Condición")]
        public CondicionCamisetaDTO CondicionCamisetaNavigation { get; set; } = new();

        [DisplayName("Equipo")]
        public EquipoDTO EquipoNavigation { get; set; } = new();

        [DisplayName("Jugador")]
        public JugadorDTO JugadorNavigation { get; set; } = new();

        [DisplayName("Propietario")]
        public UsuarioDTO UsuarioVendedorNavigation { get; set; } = new();

        [DisplayName("Estado Actual")]
        public EstadoCamisetaDTO EstadoCamisetaNavigation { get; set; } = new();

        public List<IFormFile> ImagenesCamiseta { get; set; } = new();

        public List<SubastaDTO> Subastas { get; set; } = new();

        public List<CategoriaCamisetaDTO> CategoriasCamiseta { get; set; } = new();
    }
}
