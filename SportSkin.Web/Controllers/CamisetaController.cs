using Libreria.Web.Util;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> CamisetaIndex(int idUsuario, string? filtro)
        {
            ICollection<CamisetaDTO> lista;
            ICollection<CategoriaCamisetaDTO> categoriasCamiseta = await _serviceCategoriaCamiseta.ListAsync();
            ICollection<CondicionCamisetaDTO> condicionesCamiseta = await _serviceCondicionCamiseta.ListAsync();

            lista = filtro switch
            {
                "vendidos" => await _serviceCamiseta.GetCamisetasVendidas(idUsuario),
                "ensubasta" => await _serviceCamiseta.GetCamisetasEnSubasta(idUsuario),
                "sinsubasta" => await _serviceCamiseta.GetCamisetasSinSubasta(idUsuario),
                _ => await _serviceCamiseta.ListAsyncByUser(idUsuario)
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

        [Authorize(Roles = "Vendedor")]
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
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> CamisetaCreate(CreacionCamisetaViewModel model)
        {          
            try
            {
                if (string.IsNullOrEmpty(model.EquipoAPIFootballJSON))
                    ModelState.AddModelError("Equipo", "-Debe seleccionar el equipo de la camiseta.");

                if (string.IsNullOrEmpty(model.JugadorAPIFootballJSON))
                    ModelState.AddModelError("Jugador", "-Debe seleccionar el jugador de la camiseta.");

                if (model.CamisetaDTO.Temporada == 0)
                    ModelState.AddModelError("Temporada", "-Debe seleccionar la temporada de la camiseta.");

                if (model.ImagenesCamiseta.Count == 0)
                    ModelState.AddModelError("Imágenes", "-Debe adjuntar al menos una imagen de la camiseta.");

                //Al haber errores de validación, los recopila y los muestra en el mensaje de respuesta.

                //Filtra las validaciones del modelo para obtener los mensajes de campos necesarios, dado que el modelo incluye una gran cantidad de campos que no son releventes en este momento de validar.              
                var camposAValidar = new[] { "Equipo", "Jugador", "Temporada", "Imágenes" };
                
                IEnumerable<string> mensajesError = ModelState
                                    .Where(x => camposAValidar.Contains(x.Key))
                                    .SelectMany(v => v.Value.Errors)
                                    .Select(e => e.ErrorMessage);

                //En caso de existir errores de validación, los recopila y devuelve a la interfaz.
                if (mensajesError.Any())
                {                     
                    string mensaje = "Estimado usuario, existen los siguientes errores de validación en el fomulario:<br/><br/>";

                    return Json(new
                    {
                        statusCode = "warning",
                        message = mensaje += string.Join("<br/>", mensajesError)
                                 
                    });
                }

                //Asignación del id del usuario en sesión como vendedor.
                var usuarioSesion = JObject.Parse(HttpContext.Session.GetString("UsuarioSesion") ?? "");

                if (int.TryParse(usuarioSesion["IdUsuario"]?.ToString(), out int idUsuario))
                    model.CamisetaDTO.IdUsuarioVendedor = idUsuario;
                else
                    throw new Exception("Ha ocurrido al intentar obtener el Id del usuario en sesión.");

                //Asigna las categorias seleccionadas a partir del catálogo, aplicando un filtro.
                model.CamisetaDTO.CategoriasCamiseta = (await _serviceCategoriaCamiseta.ListAsync())?.Where(x => model.CategoriasSeleccionadas.Contains(x.IdCategoriaCamiseta)).ToList() ?? [];

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
                    statusCode = "sucess",
                    message = $"La camiseta {model.CamisetaDTO.Nombre} ha sido creada satsifactoriamente."
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = "error",
                    message = $"Ha ocurrido un error al intentar crear la camiseta. Por favor intente nuevamente o contacte a soporte del sistema."
                });              
            }
        }

        [HttpGet]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> CamisetaEdit(int id)
        {
            CamisetaDTO camiseta = await _serviceCamiseta.FindByIdAsync(id);
            ICollection<CategoriaCamisetaDTO> categoriasCamiseta = await _serviceCategoriaCamiseta.ListAsync();
            ICollection<CondicionCamisetaDTO> condicionesCamiseta = await _serviceCondicionCamiseta.ListAsync();

            if (camiseta == null)
                return NotFound();

            //Lectura del usuario en un objeto dinámico de tipo JObject.
            var usuarioSesion = JObject.Parse(HttpContext.Session.GetString("UsuarioSesion") ?? "");

            CreacionCamisetaViewModel camisetaViewModel = new()
            {
                CamisetaDTO = camiseta,
                CategoriasSeleccionadas = [.. camiseta.CategoriasCamiseta.Select(x => x.IdCategoriaCamiseta)],
                EquipoAPIFootballJSON = JsonSerializer.Serialize(camiseta.EquipoNavigation),
                JugadorAPIFootballJSON = JsonSerializer.Serialize(camiseta.JugadorNavigation),
                CategoriasCamiseta = categoriasCamiseta,
                CondicionesCamiseta = condicionesCamiseta,
                NombreCompletoVendedor = $"{usuarioSesion["Nombre"]} {usuarioSesion["Apellido1"]} {usuarioSesion["Apellido2"]}"
            };

            //Obtención de imagenes en un Viewbag, con el formato manejado por FilePond.
            ViewBag.ImagenesCamiseta = camiseta.ImagenesCamiseta
                                              .Select(img => new
                                              {
                                                  source = img.RutaImagen,
                                                  options = new
                                                  {
                                                      type = "local",
                                                  }
                                              }).ToList();

            return PartialView("_CamisetaEdit", camisetaViewModel);
        }

        // POST: Camiseta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> CamisetaEdit(CreacionCamisetaViewModel model, List<int> IdsImagenesEliminadas)
        {
            try
            {
                if (string.IsNullOrEmpty(model.EquipoAPIFootballJSON))
                    ModelState.AddModelError("Equipo", "-Debe seleccionar el equipo de la camiseta.");

                if (string.IsNullOrEmpty(model.JugadorAPIFootballJSON))
                    ModelState.AddModelError("Jugador", "-Debe seleccionar el jugador de la camiseta.");

                if (model.CamisetaDTO.Temporada == 0)
                    ModelState.AddModelError("Temporada", "-Debe seleccionar la temporada de la camiseta.");

                //if (model.ImagenesCamiseta.Count == 0)
                //    ModelState.AddModelError("Imágenes", "-Debe adjuntar al menos una imagen de la camiseta.");

                //Al haber errores de validación, los recopila y los muestra en el mensaje de respuesta.

                //Filtra las validaciones del modelo para obtener los mensajes de campos necesarios, dado que el modelo incluye una gran cantidad de campos que no son releventes en este momento de validar.              
                var camposAValidar = new[] { "Equipo", "Jugador", "Temporada", "Imágenes" };

                IEnumerable<string> mensajesError = ModelState
                                    .Where(x => camposAValidar.Contains(x.Key))
                                    .SelectMany(v => v.Value.Errors)
                                    .Select(e => e.ErrorMessage);

                //En caso de existir errores de validación, los recopila y devuelve a la interfaz.
                if (mensajesError.Any())
                {
                    string mensaje = "Estimado usuario, existen los siguientes errores de validación en el fomulario:<br/><br/>";

                    return Json(new
                    {
                        statusCode = "warning",
                        message = mensaje += string.Join("<br/>", mensajesError)

                    });
                }
               
                //Asigna las categorias seleccionadas a partir del catálogo, aplicando un filtro.
                model.CamisetaDTO.CategoriasCamiseta = (await _serviceCategoriaCamiseta.ListAsync())?.Where(x => model.CategoriasSeleccionadas.Contains(x.IdCategoriaCamiseta)).ToList() ?? [];

                //Deserealiza las estructuras JSON de Equipo y Jugador en DTOS.
                model.CamisetaDTO.EquipoNavigation = JsonSerializer.Deserialize<EquipoDTO>(model.EquipoAPIFootballJSON) ?? model.CamisetaDTO.EquipoNavigation;
                model.CamisetaDTO.JugadorNavigation = JsonSerializer.Deserialize<JugadorDTO>(model.JugadorAPIFootballJSON) ?? model.CamisetaDTO.JugadorNavigation;

                //Valores por defecto al crear una nueva camiseta.               
                model.CamisetaDTO.FechaModificacion = DateTime.Now;

                await _serviceCamiseta.UpdateAsync(model.CamisetaDTO, model.ImagenesCamiseta, IdsImagenesEliminadas);

                return Json(new
                {
                    statusCode = "sucess",
                    message = $"La camiseta {model.CamisetaDTO.Nombre} ha sido editada satsifactoriamente."
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = "error",
                    message = $"Ha ocurrido un error al intentar editar la camiseta. Por favor intente nuevamente o contacte a soporte del sistema."
                });
            }
        }

        // GET: Camiseta/Delete/5      
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var camiseta = await _serviceCamiseta.FindByIdAsync(id);
        //    if (camiseta == null)
        //        return NotFound();

        //    return View(camiseta);
        //}

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> ChangeState(int id)
        {
            try
            {
                await _serviceCamiseta.ChangeStateAsync(id);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Estado actualizado",
                    text = "El estado de la camiseta fue cambiada exitosamente.",
                    icon = "success"
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

            return RedirectToAction(nameof(CamisetaIndex));
        }
    }
}
