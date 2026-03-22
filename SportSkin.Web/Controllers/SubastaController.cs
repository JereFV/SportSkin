using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Web.ViewModels;
using System.Text.Json;

namespace SportSkin.Web.Controllers
{
    public class SubastaController : Controller
    {
        private readonly IServiceSubasta _serviceSubasta;
        private readonly IServiceCamiseta _serviceCamiseta;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly SubastaSettings _settings;

        public SubastaController(
            IServiceSubasta serviceSubasta,
            IServiceCamiseta serviceCamiseta,
            IServiceUsuario serviceUsuario,
            IOptions<SubastaSettings> settings)
        {
            _serviceSubasta = serviceSubasta;
            _serviceCamiseta = serviceCamiseta;
            _serviceUsuario = serviceUsuario;
            _settings = settings.Value;
        }

        private int GetUsuarioSesionId()
        {
            var json = HttpContext.Session.GetString("UsuarioSesion") ?? "{}";
            var obj = JObject.Parse(json);
            return obj["IdUsuario"]?.Value<int>() ?? 0;
        }
        private string GetUsuarioSesionNombre()
        {
            var json = HttpContext.Session.GetString("UsuarioSesion") ?? "{}";
            var obj = JObject.Parse(json);
            return $"{obj["Nombre"]} {obj["Apellido1"]} {obj["Apellido2"]}".Trim();
        }

        // GET: Subasta/Index
        public async Task<IActionResult> SubastaIndex(string? filtro, DateTime? desde, DateTime? hasta)
        {
            ICollection<SubastaDTO> lista = filtro switch
            {
                "activas" => await _serviceSubasta.GetSubastasActivasAsync(desde, hasta),
                "finalizadas" => await _serviceSubasta.GetSubastasFinalizadasAsync(desde, hasta),
                "vendidas" => await _serviceSubasta.GetSubastasVendidasAsync(),
                _ => await _serviceSubasta.ListAsync()
            };

            ViewBag.FiltroActual = filtro;
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");

            return View(lista);
        }

        // Construye el CrearSubastaViewModel ───────
        // Centraliza la carga del formulario de creación para no repetirlo cada vez
        private async Task<CrearSubastaViewModel> ConstruirFormCrearAsync(
            CrearSubastaViewModel? vmConErrores = null)
        {
            var camisetas = await _serviceCamiseta.GetCamisetasSinSubasta();

            if (vmConErrores != null)
            {
                // Si venimos de un error, preservamos los valores ya ingresados
                vmConErrores.NombreVendedor = GetUsuarioSesionNombre();
                vmConErrores.CamisetasDisponibles = camisetas.ToList();
                return vmConErrores;
            }

            return new CrearSubastaViewModel
            {
                NombreVendedor = GetUsuarioSesionNombre(),
                CamisetasDisponibles = camisetas.ToList()
            };
        }


        // GET: Subasta/MisSubastas        
        // Subastas del vendedor en sesión + ViewModel para modal de creación
        public async Task<IActionResult> MisSubastas()
        {
            int idVendedor = GetUsuarioSesionId();

            var vm = new MisSubastasViewModel
            {
                Subastas = await _serviceSubasta.GetSubastasByVendedorAsync(idVendedor),
                IdVendedor = idVendedor,
                NombreVendedor = GetUsuarioSesionNombre(),
                FormCrear = await ConstruirFormCrearAsync()
            };

            return View(vm);
        }

        // GET: Subasta/Activas?
        public async Task<IActionResult> Activas(DateTime? desde, DateTime? hasta)
        {
            var lista = await _serviceSubasta.GetSubastasActivasAsync(desde, hasta);
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            return View(lista);
        }

        // GET: Subasta/Finalizadas?
        public async Task<IActionResult> Finalizadas(DateTime? desde, DateTime? hasta)
        {
            var lista = await _serviceSubasta.GetSubastasFinalizadasAsync(desde, hasta);
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            return View(lista);
        }

        // GET: Subasta/Details/5
        public async Task<IActionResult> Detalle(int id)
        {
            var subasta = await _serviceSubasta.FindByIdAsync(id);

            if (subasta == null)
                return NotFound();

            DetalleSubastaViewModel detalleSubastaViewModel = new DetalleSubastaViewModel()
            {
                Subasta = subasta,
                SituacionFirma = subasta.IdCamisetaNavigation.Autografiada ? "Firmada" : "No Firmada",
                PujaActual = subasta.Puja.Any() ? subasta.Puja.Max(x => x.Monto) : subasta.PrecioBase,
                CantidadTotalPujas = subasta.Puja?.Count ?? 0
            };

            return View(detalleSubastaViewModel);
        }

        // ────────────────────────────────────────────────────────────
        // POST AJAX: /Subasta/Crear
        //
        // Devuelve SIEMPRE JSON — nunca redirect ni View.
        // El JS es quien recarga la página si success = true.
        // Así AJAX funciona correctamente sin recibir HTML inesperado.
        // ────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearSubastaViewModel vm)
        {
            // Validación de fecha: no está cubierta por DataAnnotations
            if (vm.FechaCierre <= vm.FechaInicio)
                ModelState.AddModelError(nameof(vm.FechaCierre),
                    "La fecha de cierre debe ser posterior a la fecha de inicio.");

            // Si el ModelState no es válido devolver los errores como JSON
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, errors = errores });
            }

            try
            {
                var dto = new SubastaDTO
                {
                    IdCamiseta = vm.IdCamiseta,
                    FechaInicio = vm.FechaInicio,
                    FechaCierre = vm.FechaCierre,
                    PrecioBase = vm.PrecioBase,
                    IncrementoMinimo = vm.IncrementoMinimo,
                    PrecioCompraInmediata = vm.PrecioCompraInmediata,
                    IdEstadoSubasta = 4  // Borrador
                };

                await _serviceSubasta.AddAsync(dto);

                // En AJAX no se usa TempData para notificaciones porque
                // la página se recarga desde JS. El mensaje va en el JSON.
                return Json(new
                {
                    success = true,
                    message = "La subasta fue guardada como Borrador."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, errors = new[] { ex.Message } });
            }
            catch (Exception ex)
            {
                // Temporalmente mostrar el error completo para diagnosticar
                return Json(new
                {
                    success = false,
                    errors = new[] { ex.Message + " | INNER: " + ex.InnerException?.Message + " | " + ex.InnerException?.InnerException?.Message }
                });
            }
        }

        // ─────────────────────────────────────────────────────────────
        // GET: /Subasta/GetDatosEditar/5  (AJAX — llena modal de edición)
        // ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetDatosEditar(int id)
        {
            var subasta = await _serviceSubasta.FindByIdAsync(id);
            if (subasta == null)
                return NotFound();

            if (subasta.FechaInicio <= DateTime.Now)
                return BadRequest("La subasta ya ha iniciado y no puede editarse.");

            if (subasta.Puja.Any())
                return BadRequest("La subasta tiene pujas y no puede editarse.");

            var vendedor = subasta.IdCamisetaNavigation?.UsuarioVendedorNavigation;
            var nombreVendedor = vendedor != null
                ? $"{vendedor.Nombre} {vendedor.Apellido1} {vendedor.Apellido2}".Trim()
                : GetUsuarioSesionNombre();

            return Json(new
            {
                idSubasta = subasta.IdSubasta,
                idCamiseta = subasta.IdCamiseta,
                nombreCamiseta = subasta.IdCamisetaNavigation?.Nombre ?? "—",
                estadoActual = subasta.IdEstadoSubastaNavigation?.Nombre ?? "—",
                nombreVendedor = nombreVendedor,
                fechaInicio = subasta.FechaInicio.ToString("yyyy-MM-ddTHH:mm"),
                fechaCierre = subasta.FechaCierre.ToString("yyyy-MM-ddTHH:mm"),
                precioBase = subasta.PrecioBase,
                incrementoMinimo = subasta.IncrementoMinimo,
                precioCompraInmediata = subasta.PrecioCompraInmediata
            });
        }

        // ─────────────────────────────────────────────────────────────
        // POST: /Subasta/Editar
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Editar(EditarSubastaViewModel vm)
        {
            if (vm.FechaCierre <= vm.FechaInicio)
                ModelState.AddModelError("FechaCierre",
                    "La fecha de cierre debe ser posterior a la fecha de inicio.");

            if (!ModelState.IsValid)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Validación",
                    text = "Revise los campos del formulario de edición.",
                    icon = "warning"
                });
                return RedirectToAction(nameof(MisSubastas));
            }

            try
            {
                var dto = new SubastaDTO
                {
                    IdSubasta = vm.IdSubasta,
                    IdCamiseta = vm.IdCamiseta,
                    FechaInicio = vm.FechaInicio,
                    FechaCierre = vm.FechaCierre,
                    PrecioBase = vm.PrecioBase,
                    IncrementoMinimo = vm.IncrementoMinimo,
                    PrecioCompraInmediata = vm.PrecioCompraInmediata,
                    IdEstadoSubasta = 5
                };

                await _serviceSubasta.UpdateAsync(vm.IdSubasta, dto);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Subasta actualizada",
                    text = "Los cambios fueron guardados correctamente.",
                    icon = "success"
                });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "No se pudo editar",
                    text = ex.Message,
                    icon = "warning"
                });
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Error inesperado",
                    text = ex.Message,
                    icon = "error"
                });
            }

            return RedirectToAction(nameof(MisSubastas));
        }

        // ─────────────────────────────────────────────────────────────
        // POST: /Subasta/Publicar/5
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Publicar(int id)
        {
            try
            {
                await _serviceSubasta.PublicarAsync(id);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Subasta publicada",
                    text = "La subasta ahora está En proceso y es visible.",
                    icon = "success"
                });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "No se pudo publicar",
                    text = ex.Message,
                    icon = "warning"
                });
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Error inesperado",
                    text = ex.Message,
                    icon = "error"
                });
            }

            return RedirectToAction(nameof(MisSubastas));
        }

        // ─────────────────────────────────────────────────────────────
        // POST: /Subasta/Cancelar/5
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                await _serviceSubasta.CancelarAsync(id);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Subasta cancelada",
                    text = "La subasta fue cancelada correctamente.",
                    icon = "info"
                });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "No se puede cancelar",
                    text = ex.Message,
                    icon = "warning"
                });
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Error inesperado",
                    text = ex.Message,
                    icon = "error"
                });
            }

            return RedirectToAction(nameof(MisSubastas));
        }

    }
}
