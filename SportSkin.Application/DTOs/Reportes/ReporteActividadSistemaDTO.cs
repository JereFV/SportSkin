using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.DTOs
{
    /// <summary>
    /// Reporte 4: Actividad general del sistema por periodo.
    /// </summary>
    public class ReporteActividadSistemaDTO
    {
        /// <summary>Totales del periodo completo para las métricas resumen.</summary>
        public int TotalSubastasCreadas { get; set; }
        public int TotalPujasRealizadas { get; set; }
        public int TotalSubastasFinalizadas { get; set; }

        /// <summary>Detalle desglosado por periodo (semana / mes / trimestre).</summary>
        public List<PeriodoActividadDTO> Periodos { get; set; } = new();
    }

    /// <summary>Un punto de datos en la granularidad elegida.</summary>
    public class PeriodoActividadDTO
    {
        /// <summary>Etiqueta legible del periodo, ej. "Ene 2024", "Q1 2024", "Sem 3".</summary>
        public string Label { get; set; } = string.Empty;

        public int SubastasCreadas { get; set; }
        public int PujasRealizadas { get; set; }
        public int SubastasFinalizadas { get; set; }
    }
}
