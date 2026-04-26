using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    public record UsuarioDTO
    {
        [DisplayName("Identificador Usuario")]
        public int IdUsuario { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Primer Apellido")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Apellido1 { get; set; } = string.Empty;

        [DisplayName("Segundo Apellido")]
        [StringLength(100, ErrorMessage = "{0} no puede superar {1} caracteres")]
        public string? Apellido2 { get; set; }

        [DisplayName("Rol")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(1, byte.MaxValue, ErrorMessage = "Debe seleccionar un {0}")]
        public byte IdRolUsuario { get; set; }

        [DisplayName("Teléfono")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Phone(ErrorMessage = "{0} no tiene un formato válido")]
        public string Telefono { get; set; } = string.Empty;

        [DisplayName("Correo Electrónico")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [EmailAddress(ErrorMessage = "{0} no tiene un formato válido")]
        public string Correo { get; set; } = string.Empty;

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        [DisplayName("Usuario")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Usuario1 { get; set; } = string.Empty;

        [DisplayName("Contraseña")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres")]
        public string Contrasenna { get; set; } = string.Empty;

        public byte? IdPreguntaRecuperacion { get; set; }

        public string? RespuestaPreguntaRecuperacion { get; set; }

        // Navegación
        [DisplayName("Rol")]
        public RolUsuarioDTO RolUsuarioNavigation { get; set; } = new();

        public List<SubastaDTO> Subasta { get; set; } = new();  // subastas como vendedor

        public List<PujaDTO> Puja { get; set; } = new(); //subastas como comprador

        public List<CamisetaDTO> Camiseta { get; set; } = new();

        public PreguntaRecuperacionUsuarioDTO? PreguntaRecuperacionNavigation { get; set; } = new();
    }
}
