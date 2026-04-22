using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    /// <summary>
    /// Reporte 1: Participación de usuarios compradores en subastas.
    /// Una fila por usuario, considerando únicamente pujas válidas.
    /// </summary>
    public class ReporteParticipacionCompradorDTO
    {
        [DisplayName("Identificador Usuario")]
        public int IdUsuario { get; set; }

        [DisplayName("Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [DisplayName("Correo")]
        public string Correo { get; set; } = string.Empty;

        [DisplayName("Subastas en las que participó")]
        public int CantidadSubastas { get; set; }

        [DisplayName("Total de pujas realizadas")]
        public int TotalPujas { get; set; }

        [DisplayName("Pujas ganadoras")]
        public int PujasGanadoras { get; set; }

        /// <summary>
        /// Porcentaje de subastas en las que el usuario fue el mayor postor al cierre.
        /// </summary>
        [DisplayName("Tasa de éxito (%)")]
        public double TasaExito => CantidadSubastas > 0
            ? Math.Round((double)PujasGanadoras / CantidadSubastas * 100, 1)
            : 0;

        [DisplayName("Última actividad")]
        public DateTime? UltimaActividad { get; set; }
    }
}
