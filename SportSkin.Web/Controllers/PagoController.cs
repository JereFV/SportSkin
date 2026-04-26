using Libreria.Web.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Web.ViewModels;
using System.Text.Json;

namespace SportSkin.Web.Controllers
{
    public class PagoController : Controller
    {
        private readonly IServicePago _servicePago;
        private readonly IServiceMetodoPago _serviceMetodoPago;

        public PagoController(IServicePago servicePago, IServiceMetodoPago serviceMetodoPago)
        {
            _servicePago = servicePago;
            _serviceMetodoPago = serviceMetodoPago;
        }

        // GET: Pago/PagoIndex
        [Authorize(Roles = "Administrador,Comprador")]
        public async Task<IActionResult> PagoIndex()
        {
            var sesionStr = HttpContext.Session.GetString("UsuarioSesion");
            var sesion = sesionStr != null ? JObject.Parse(sesionStr) : null;
            var rol = sesion != null ? (int?)sesion["IdRolUsuario"] : null;
            var idUsuario = sesion != null ? (int?)sesion["IdUsuario"] : null;

            // Vendedor no tiene acceso a pagos
            if (rol == 2)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Acceso denegado",
                    "Los vendedores no tienen acceso al módulo de pagos.",
                    SweetAlertMessageType.warning);
                return RedirectToAction("HomeIndex", "Home");
            }

            var facturas = await _servicePago.ListAsync();
            var metodos = await _serviceMetodoPago.ListAsync();
            var subastasPendientes = await _servicePago.GetSubastasPendientesPagoAsync();

            // Comprador (rol 3): filtra solo sus propios pagos y subastas pendientes
            if (rol == 3 && idUsuario.HasValue)
            {
                facturas = facturas
                    .Where(f => f.IdSubastaNavigation?.IdUsuarioComprador == idUsuario.Value)
                    .ToList();

                subastasPendientes = subastasPendientes
                    .Where(s => s.IdUsuarioComprador == idUsuario.Value)
                    .ToList();
            }

            var pagos = facturas.Select(f =>
            {
                var comprador = f.IdSubastaNavigation?.IdUsuarioCompradorNavigation;
                var camiseta = f.IdSubastaNavigation?.IdCamisetaNavigation;

                return new ListadoPagoViewModel
                {
                    IdFactura = f.IdFactura,
                    IdSubasta = f.IdSubasta ?? 0,
                    NombreCamiseta = camiseta?.Nombre ?? "—",
                    RutaImagenCamiseta = camiseta?.ImagenesCamiseta?.FirstOrDefault()?.RutaImagen ?? string.Empty,
                    NombreComprador = comprador != null
                        ? $"{comprador.Nombre} {comprador.Apellido1} {comprador.Apellido2}".Trim()
                        : "—",
                    Total = f.Total,
                    FechaCreacion = f.FechaCreacion,
                    FechaPago = f.FechaPago,
                    EstadoPago = f.IdEstadoFacturaNavigation?.Nombre ?? "—",
                    IdEstadoFactura = f.IdEstadoFactura,
                    MetodoPago = f.IdMetodoPagoNavigation?.Nombre ?? "—"
                };
            }).ToList();

            var pendientes = subastasPendientes.Select(s =>
            {
                var comprador = s.IdUsuarioCompradorNavigation;
                var camiseta = s.IdCamisetaNavigation;

                return new SubastaPendientePagoViewModel
                {
                    IdSubasta = s.IdSubasta,
                    NombreCamiseta = camiseta?.Nombre ?? "—",
                    RutaImagen = camiseta?.ImagenesCamiseta?.FirstOrDefault()?.RutaImagen ?? string.Empty,
                    NombreComprador = comprador != null
                        ? $"{comprador.Nombre} {comprador.Apellido1} {comprador.Apellido2}".Trim()
                        : "—",
                    MontoTotal = s.MontoCompra ?? 0,
                    FechaCierre = s.FechaCierre
                };
            }).ToList();

            var vm = new PagoIndexViewModel
            {
                Pagos = pagos,
                MetodosPago = metodos,
                SubastasPendientes = pendientes
            };

            return View(vm);
        }

        // POST: Pago/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comprador")]
        public async Task<IActionResult> Registrar(int idSubasta, byte idMetodoPago)
        {
            try
            {
                var idFactura = await _servicePago.RegistrarAsync(idSubasta, idMetodoPago);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Pago Registrado",
                    $"La factura #{idFactura} fue registrada con estado Pendiente.",
                    SweetAlertMessageType.success);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Aviso", ex.Message, SweetAlertMessageType.warning);
            }
            catch (Exception)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", "Ocurrió un error al registrar el pago.", SweetAlertMessageType.error);
            }

            return RedirectToAction(nameof(PagoIndex));
        }

        // POST: Pago/Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comprador")]
        public async Task<IActionResult> Confirmar(string idFactura)
        {
            try
            {
                await _servicePago.ConfirmarAsync(idFactura);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Pago Confirmado",
                    $"La factura #{idFactura} fue confirmada exitosamente.",
                    SweetAlertMessageType.success);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Aviso", ex.Message, SweetAlertMessageType.warning);
            }
            catch (Exception)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", "Ocurrió un error al confirmar el pago.", SweetAlertMessageType.error);
            }

            return RedirectToAction(nameof(PagoIndex));
        }
    }
}