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
        [Required(ErrorMessage ="El nombre de la camiseta es un dato requerido.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre de la camiseta debe tener entre {2} y {1} caracteres.")]
        public string Nombre {  get; set; } = string.Empty;

        [DisplayName("Descripción")]
        [Required(ErrorMessage ="La descripción es un dato requerido")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "La descripción debe tener entre {2} y {1} caracteres.")]
        public string Descripcion {  get; set; } = string.Empty;

        [Required(ErrorMessage = "La condición de la camiseta es un dato requerido.")]
        public byte IdCondicionCamiseta { get; set; }

        public short IdEquipo {  get; set; }

        public int IdJugador {  get; set; }

        [DisplayName("Temporada")]       
        public short Temporada { get; set; }

        [DisplayName("¿Autografiada?")]
        public bool Autografiada { get; set; }

        public int IdUsuarioVendedor { get; set; }

        public byte IdEstadoCamiseta { get; set; }

        public bool EstadoRegistro { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }

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

        public List<ImagenCamisetaDTO> ImagenesCamiseta { get; set; } = new();

        public List<SubastaDTO> Subastas { get; set; } = new();

        public List<CategoriaCamisetaDTO> CategoriasCamiseta { get; set; } = new();
    }
}
