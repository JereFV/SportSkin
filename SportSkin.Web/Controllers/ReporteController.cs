using Libreria.Web.Util;
using Microsoft.AspNetCore.Mvc;
using SportSkin.Application.Services.Interfaces;

namespace SportSkin.Web.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IServiceSubasta _serviceSubasta;

        public ReporteController(IServiceSubasta serviceSubasta)
        {
            _serviceSubasta = serviceSubasta;
        }

        // ── Reporte 1 ──────────────────────────────────────────────────────
        // GET: /Reporte/ReporteParticipacionCompradores
        public IActionResult ReporteParticipacionCompradores()
        {
            return View();
        }

        // GET: /Reporte/GetParticipacionCompradores?desde=2024-01-01&hasta=2024-12-31
        [HttpGet]
        public async Task<IActionResult> GetParticipacionCompradores(DateTime? desde, DateTime? hasta)
        {
            try
            {
                var datos = await _serviceSubasta.GetParticipacionCompradoresAsync(desde, hasta);
                return Json(datos);
            }
            catch (Exception)
            {
                ViewBag.Exception = SweetAlertHelper.CrearNotificacion(
                    "Reporte 1",
                    "Ha ocurrido un error al generar el reporte de participación.",
                    SweetAlertMessageType.error);
                return StatusCode(500, new { mensaje = "Error al obtener los datos del reporte." });
            }
        }

        // ── Reporte 4 ──────────────────────────────────────────────────────
        // GET: /Reporte/ReporteActividadSistema
        public IActionResult ReporteActividadSistema()
        {
            return View();
        }

        // GET: /Reporte/GetActividadSistema?desde=2024-01-01&hasta=2024-12-31&granularidad=mensual
        [HttpGet]
        public async Task<IActionResult> GetActividadSistema(
            DateTime? desde, DateTime? hasta, string granularidad = "mensual")
        {
            // Valores por defecto: último año completo
            var fechaDesde = desde ?? new DateTime(DateTime.Now.Year - 1, 1, 1);
            var fechaHasta = hasta ?? DateTime.Now;

            try
            {
                var datos = await _serviceSubasta.GetActividadSistemaAsync(
                    fechaDesde, fechaHasta, granularidad);
                return Json(datos);
            }
            catch (Exception)
            {
                ViewBag.Exception = SweetAlertHelper.CrearNotificacion(
                    "Reporte 4",
                    "Ha ocurrido un error al generar el reporte de actividad.",
                    SweetAlertMessageType.error);
                return StatusCode(500, new { mensaje = "Error al obtener los datos del reporte." });
            }
        }
    }
}
