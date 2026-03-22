using Libreria.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Web.ViewModels;
using System.Globalization;
using System.Text.Json;

namespace SportSkin.Web.Controllers
{
    public class CamisetaController : Controller
    {
        private readonly IServiceCamiseta _serviceCamiseta;
        private readonly IServiceCategoriaCamiseta _serviceCategoriaCamiseta;
        private readonly IServiceCondicionCamiseta _serviceCondicionCamiseta;
        private readonly IServiceEquipo _serviceEquipo;
        private readonly IServiceJugador _serviceJugador;
        private readonly IServiceTrayectoriaJugador _serviceTrayectoriaJugador;

        public CamisetaController(IServiceCamiseta service, IServiceCategoriaCamiseta serviceCategoriaCamiseta, IServiceCondicionCamiseta serviceCondicionCamiseta, IServiceEquipo serviceEquipo, IServiceJugador serviceJugador, IServiceTrayectoriaJugador serviceTrayectoriaJugador)
        {
            _serviceCamiseta = service;
            _serviceCategoriaCamiseta = serviceCategoriaCamiseta;
            _serviceCondicionCamiseta = serviceCondicionCamiseta;
            _serviceEquipo = serviceEquipo;
            _serviceJugador = serviceJugador;
            _serviceTrayectoriaJugador = serviceTrayectoriaJugador;
        }

        // GET: Camiseta
        public async Task<IActionResult> CamisetaIndex(string? filtro)
        {
            ICollection<CamisetaDTO> lista;
            ICollection<CategoriaCamisetaDTO> categoriasCamiseta = await _serviceCategoriaCamiseta.ListAsync();
            ICollection<CondicionCamisetaDTO> condicionesCamiseta = await _serviceCondicionCamiseta.ListAsync();

            lista = filtro switch
            {
                "vendidos" => await _serviceCamiseta.GetCamisetasVendidas(),
                "ensubasta" => await _serviceCamiseta.GetCamisetasEnSubasta(),
                "sinsubasta" => await _serviceCamiseta.GetCamisetasSinSubasta(),
                _ => await _serviceCamiseta.ListAsync()
            };

            //Lectura del usuario en un objeto dinámico de tipo JObject.
            var usuarioSesion = JObject.Parse(HttpContext.Session.GetString("UsuarioSesion") ?? "");

            //Creación de ViewModel general para la pantalla de camisetas.
            CamisetaViewModel camisetaViewModel = new()
            {
                Camisetas = lista,
                CreacionCamiseta = new CreacionCamisetaViewModel
                {
                    CategoriasCamiseta = categoriasCamiseta,
                    CondicionesCamiseta = condicionesCamiseta,
                    NombreCompletoVendedor = $"{usuarioSesion["Nombre"]} {usuarioSesion["Apellido1"]} {usuarioSesion["Apellido2"]}"
                }
            };

            ViewBag.FiltroActual = filtro;

            return View(camisetaViewModel);
        }

        // GET: Camiseta/Details/5
        public async Task<IActionResult> CamisetaDetails(int id)
        {
            var camiseta = await _serviceCamiseta.FindByIdAsync(id);

            if (camiseta == null)
                return NotFound();

            //Creación de entidad ViewModel.
            DetalleCamisetaViewModel detalleCamisetaViewModel = new DetalleCamisetaViewModel()
            {
                Camiseta = camiseta,
                NombreCompletoPropietario = $"{camiseta.UsuarioVendedorNavigation?.Nombre} {camiseta.UsuarioVendedorNavigation?.Apellido1} {camiseta.UsuarioVendedorNavigation?.Apellido2}",
                FechaRegistroFormateada = camiseta.FechaRegistro.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                SituacionFirma = camiseta.Autografiada ? "Firmada" : "No Firmada"
            };

            return View(detalleCamisetaViewModel);
        }

        // POST: Camiseta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CamisetaCreate(CreacionCamisetaViewModel model)
        {          
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                //        "Advertencia de validación", 
                //        "El formulario contiene errores en los valores de los campos, por favor realizar las correciones e intentar nuevamente.", 
                //        SweetAlertMessageType.warning);

                //    return View("_CamisetaCreate", model);
                //}

                //Asignación del id del usuario en sesión como vendedor.
                var usuarioSesion = JObject.Parse(HttpContext.Session.GetString("UsuarioSesion") ?? "");

                if (int.TryParse(usuarioSesion["IdUsuario"]?.ToString(), out int idUsuario))
                    model.CamisetaDTO.IdUsuarioVendedor = idUsuario;
                else
                    throw new Exception("Ha ocurrido al intentar el Id del usuario en sesión.");

                //Asigna las categorias seleccionadas a partir del catálogo, aplicando un filtro.
                model.CamisetaDTO.CategoriasCamiseta = model.CategoriasCamiseta?.Where(x => model.CategoriasSeleccionadas.Contains(x.IdCategoriaCamiseta)).ToList() ?? [];

                //Deserealiza las estructuras JSON de Equipo y Jugador en DTOS.
                model.CamisetaDTO.EquipoNavigation = JsonSerializer.Deserialize<EquipoDTO>(model.EquipoAPIFootballJSON) ?? model.CamisetaDTO.EquipoNavigation;
                model.CamisetaDTO.JugadorNavigation = JsonSerializer.Deserialize<JugadorDTO>(model.JugadorAPIFootballJSON) ?? model.CamisetaDTO.JugadorNavigation;

                //Valores por defecto al crear una nueva camiseta.
                model.CamisetaDTO.IdEstadoCamiseta = 1; //Disponible, estado inicial por defecto.
                model.CamisetaDTO.EstadoRegistro = true;
                model.CamisetaDTO.FechaRegistro = DateTime.Now;

                await _serviceCamiseta.AddAsync(model.CamisetaDTO, model.ImagenesCamiseta);

                return Json(new
                {
                    sucess = true,
                    message = $"La camiseta {model.CamisetaDTO.Nombre} ha sido creada satsifactoriamente."
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    sucess = false,
                    message = $"Ha ocurrido un error al intentar crear la camiseta. Por favor intente nuevamente o contacte a soporte del sistema."
                });
            }
        }

        // GET: Camiseta/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var camiseta = await _serviceCamiseta.FindByIdAsync(id);
            if (camiseta == null)
                return NotFound();

            return View(camiseta);
        }

        // POST: Camiseta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CamisetaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _serviceCamiseta.UpdateAsync(id, dto);
                TempData["success"] = "Camiseta actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // GET: Camiseta/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var camiseta = await _serviceCamiseta.FindByIdAsync(id);
            if (camiseta == null)
                return NotFound();

            return View(camiseta);
        }

        // POST: Camiseta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _serviceCamiseta.DeleteAsync(id);
                TempData["success"] = "Camiseta eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListEquipos(string filtro)
        {
            try
            {
                var equipos = await _serviceEquipo.ListEquiposFromAPIAsync(filtro);

                return Ok(equipos);
            }
            catch (Exception)
            {
                ViewBag.Exception = SweetAlertHelper.CrearNotificacion("Crear Camiseta", "Ha ocurrido un error al intentar obtener el listado de equipos.", SweetAlertMessageType.error);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListJugadores(string filtro, int idEquipo)
        {
            try
            {
                var jugadores = await _serviceJugador.ListJugadoresFromAPIAsync(filtro, idEquipo);

                return Ok(jugadores);
            }
            catch (Exception)
            {
                ViewBag.Exception = SweetAlertHelper.CrearNotificacion("Crear Camiseta", "Ha ocurrido un error al intentar obtener el listado de jugadores para el equipo seleccionado.", SweetAlertMessageType.error);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListTemporadas(int idEquipo, int idJugador)
        {
            try
            {
                var temporadas = await _serviceTrayectoriaJugador.ListTemporadasJugadorByTeamAsync(idEquipo, idJugador);

                return Ok(temporadas);
            }
            catch (Exception)
            {
                ViewBag.Exception = SweetAlertHelper.CrearNotificacion("Crear Camiseta", "Ha ocurrido un error al intentar obtener el listado de temporadas para el jugador y equipo seleccionado.", SweetAlertMessageType.error);
                return BadRequest();
            }
        }
    }
}
