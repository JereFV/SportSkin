using Libreria.Web.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Implementations;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Models;
using SportSkin.Web.ViewModels;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace SportSkin.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServicePreguntaRecuperacionUsuario _servicePreguntaRecuperacion;

        public UsuarioController(IServiceUsuario service, IServicePreguntaRecuperacionUsuario servicePreguntaRecuperacion)
        {
            _serviceUsuario = service;
            _servicePreguntaRecuperacion = servicePreguntaRecuperacion;
        }

        //Obtiene usuario en sesión
        private int GetUsuarioSesionId()
        {
            var json = HttpContext.Session.GetString("UsuarioSesion") ?? "{}";
            var obj = JObject.Parse(json);
            return obj["IdUsuario"]?.Value<int>() ?? 0;
        }

        // GET: UsuarioController
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> UsuarioIndex()
        {
            try
            {
                var usuarios = await _serviceUsuario.ListAsync();
                var usuariosView = new List<ListadoUsuariosViewModel>();

                if (usuarios != null)
                {
                    //Construcción de una colección de tipo ViewModel según el formato esperado por el listado.
                    foreach (var usuario in usuarios)
                    {
                        var usuarioView = new ListadoUsuariosViewModel()
                        {
                            IdUsuario = usuario.IdUsuario,
                            NombreCompleto = $"{usuario.Nombre} {usuario.Apellido1} {usuario.Apellido2}",
                            Rol = usuario.RolUsuarioNavigation?.Nombre,
                            Estado = usuario.Estado ? "Activo" : "Inactivo"
                        };

                        usuariosView.Add(usuarioView);
                    }
                }
                var roles = await _serviceUsuario.GetRolesAsync();
                ViewBag.CrearUsuarioVM = new CrearUsuarioViewModel
                {
                    Roles = roles.Select(r => new SelectListItem
                    {
                        Value = r.IdRolUsuario.ToString(),
                        Text = r.Nombre
                    }).ToList()
                };

                return View(usuariosView);
            }
            catch (Exception)
            {
                ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion("Listado de usuarios", "Ha ocurrido un error al intentar obtener el listado de usuarios.", SweetAlertMessageType.error);
                throw;
            }        
        }

        // GET: UsuarioController/Details/       
        public async Task<IActionResult> UsuarioDetails(int id)
        {
            var usuario = await _serviceUsuario.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            if (usuario.IdRolUsuario == 2) // Vendedor
            {
                var stats = await _serviceUsuario.GetEstadisticasVendedorAsync(id);
                ViewBag.TotalSubastas = stats.total;
                ViewBag.SubastasActivas = stats.activas;
                ViewBag.SubastasVendidas = stats.vendidas;
                ViewBag.SubastasFinalizadas = stats.finalizadas;
            }

            return View(usuario);
        }

        // ─────────────────────────────────────────────────────────────
        // POST: /Usuario/Crear
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear(CrearUsuarioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Reconstruir vista con errores y modal abierto
                return await ReconstruirVistaCrear(vm);
            }

            try
            {
                var dto = new UsuarioDTO
                {
                    Nombre = vm.Nombre,
                    Apellido1 = vm.Apellido1,
                    Apellido2 = vm.Apellido2,
                    IdRolUsuario = vm.IdRolUsuario,
                    Telefono = vm.Telefono,
                    Correo = vm.Correo,
                    Usuario1 = vm.Usuario1,
                    Contrasenna = vm.Contrasenna,
                    Estado = true
                };

                await _serviceUsuario.AddAsync(dto);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Usuario creado",
                    text = $"El usuario '{vm.Nombre} {vm.Apellido1}' fue registrado correctamente.",
                    icon = "success"
                });
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (correo/usuario duplicado)
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "No se pudo crear",
                    text = ex.Message,
                    icon = "warning"
                });

                //Se reconstruye el model
                return await ReconstruirVistaCrear(vm);
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

            return RedirectToAction(nameof(UsuarioIndex));
        }

        //Re construye la vista sobre todo se utiliza en casos donde una validación no se cumple
        private async Task<IActionResult> ReconstruirVistaCrear(CrearUsuarioViewModel vm)
        {
            var usuarios = await _serviceUsuario.ListAsync();
            var listView = usuarios.Select(u => new ListadoUsuariosViewModel
            {
                IdUsuario = u.IdUsuario,
                NombreCompleto = $"{u.Nombre} {u.Apellido1} {u.Apellido2}".Trim(),
                Rol = u.RolUsuarioNavigation?.Nombre ?? "-",
                Estado = u.Estado ? "Activo" : "Inactivo"
            }).ToList();

            var roles = await _serviceUsuario.GetRolesAsync();
            vm.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.IdRolUsuario.ToString(),
                Text = r.Nombre,
                Selected = r.IdRolUsuario == vm.IdRolUsuario
            }).ToList();

            ViewBag.CrearUsuarioVM = vm;
            ViewBag.AbrirModalCrear = true;
            return View("UsuarioIndex", listView);
        }

        private async Task<IActionResult> ReconstruirVistaEditar(EditarUsuarioViewModel vm)
        {
            // Cargamos la lista de la tabla para que el fondo no se vea vacío
            var usuarios = await _serviceUsuario.ListAsync();
            var listView = usuarios.Select(u => new ListadoUsuariosViewModel
            {
                IdUsuario = u.IdUsuario,
                NombreCompleto = $"{u.Nombre} {u.Apellido1} {u.Apellido2}".Trim(),
                Rol = u.RolUsuarioNavigation?.Nombre ?? "-",
                Estado = u.Estado ? "Activo" : "Inactivo"
            }).ToList();

            // Necesitamos recargar también el ViewModel de "Crear" porque el Index lo espera
            var roles = await _serviceUsuario.GetRolesAsync();
            ViewBag.CrearUsuarioVM = new CrearUsuarioViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.IdRolUsuario.ToString(),
                    Text = r.Nombre
                }).ToList()
            };

            // Pasamos los datos del usuario que estamos editando
            ViewBag.EditarUsuarioVM = vm;
            ViewBag.AbrirModalEditar = true; // Esta bandera activa el JS en la vista

            return View("UsuarioIndex", listView);
        }

        // GET: /Usuario/GetDatosEditar/5  (AJAX — llena el modal editar)
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetDatosEditar(int id)
        {
            var usuario = await _serviceUsuario.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            return Json(new
            {
                idUsuario = usuario.IdUsuario,
                nombre = usuario.Nombre,
                apellido1 = usuario.Apellido1,
                apellido2 = usuario.Apellido2 ?? string.Empty,
                correo = usuario.Correo,
                telefono = usuario.Telefono,
                rol = usuario.RolUsuarioNavigation?.Nombre ?? "-",
                fechaCreacion = usuario.FechaCreacion.ToString("dd/MM/yyyy")
            });
        }
         
        // POST: /Usuario/Editar         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EditarUsuarioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return await ReconstruirVistaEditar(vm);
            }

            try
            {
                var dto = new UsuarioDTO
                {
                    IdUsuario = vm.IdUsuario,
                    Nombre = vm.Nombre,
                    Apellido1 = vm.Apellido1,
                    Apellido2 = vm.Apellido2,
                    Correo = vm.Correo,
                    Telefono = vm.Telefono
                };

                await _serviceUsuario.UpdateAsync(vm.IdUsuario, dto);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Perfil actualizado",
                    text = "Los datos del usuario fueron modificados correctamente.",
                    icon = "success"
                });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "No se pudo actualizar",
                    text = ex.Message,
                    icon = "warning"
                });
                return await ReconstruirVistaEditar(vm);
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

            if (vm.EsPerfil)
                return RedirectToAction("HomeIndex", "Home");
            else
                return RedirectToAction(nameof(UsuarioIndex));
        }

        // POST: /Usuario/ChangeStateAsync/        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ChangeState(int id)
        {
            try
            {
                await _serviceUsuario.ChangeStateAsync(id);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Estado actualizado",
                    text = "El estado del usuario fue cambiado correctamente.",
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

            return RedirectToAction(nameof(UsuarioIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarContrasenna(CambiarContrasennaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Validación",
                    text = "Revise los campos del formulario.",
                    icon = "warning"
                });
                return RedirectToAction(nameof(UsuarioIndex));
            }

            try
            {
                await _serviceUsuario.ChangePasswordAsync(vm.IdUsuario, vm.NuevaContrasenna);

                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Contraseña actualizada",
                    text = "La contraseña fue cambiada correctamente.",
                    icon = "success"
                });
            }
            catch (KeyNotFoundException)
            {
                TempData["Notificacion"] = JsonSerializer.Serialize(new
                {
                    title = "Error",
                    text = "El usuario no fue encontrado.",
                    icon = "error"
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

            return RedirectToAction(nameof(UsuarioIndex));
        }

        [HttpGet]
        public async Task<IActionResult> ListPreguntasRecuperacion()
        {
            try
            {
                var preguntas = await _servicePreguntaRecuperacion.ListAsync();

                return Ok(preguntas);
            }
            catch (Exception)
            {
                ViewBag.Exception = SweetAlertHelper.CrearNotificacion("Registro Usuario", "Ha ocurrido un error al intentar obtener el listado de preguntas de recuperación seleccionables para un nuevo usuario.", SweetAlertMessageType.error);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody]UsuarioDTO dto)
        {            
            try
            {               
                await _serviceUsuario.AddAsync(dto);

                return Json(new
                {
                    statusCode = "success",
                    message = $"¡El usuario {dto.Usuario1} ha sido registrado satisfactoriamente!"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    statusCode = "warning",
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = "error",
                    message = "Ha ocurrido un error inesperado al intentar registrar el usuario. Por favor intente nuevamente más tarde."
                });
            }           
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserName(string usuario)
        {
            try
            {
                var usuarioBuscado = await _serviceUsuario.FindByUserAsync(usuario);

                if (usuarioBuscado == null)
                    return Json(new
                    {
                        statusCode = "warning",
                        message = "El usuario digitado no se encuentra registrado en el sistema. Por favor digite otro valor."
                    });

                return Json(new
                {
                    statusCode = "success",
                    usuario = usuarioBuscado
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = "error",
                    message = "Ha ocurrido un error inesperado al intentar verificar la identidad del usuario digitado. Por favor intente nuevamente más tarde."
                });
            }
        }

        [HttpPost]       
        public async Task<IActionResult> RecuperarContrasenna([FromBody]UsuarioDTO dto)
        {        
            try
            {
                await _serviceUsuario.ChangePasswordAsync(dto.IdUsuario, dto.Contrasenna);

                return Json(new
                {
                    statusCode = "success",
                    message = "¡Contraseña actualizada satisfactoriamente!"
                });
            }          
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = "error",
                    message = "Ha ocurrido un error inesperado al intentar actualizar su contraseña. Por favor intente de nuevo más tarde."
                });
            }        
        }

        //Obtiene los datos del usuario en sesión.
        public async Task<IActionResult> GetPerfilUsuario()
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var usuario = await _serviceUsuario.FindByIdAsync(idUsuario);

            if (usuario == null)
                return NotFound();

            return Json(new
            {
                idUsuario = usuario.IdUsuario,
                nombre = usuario.Nombre,
                apellido1 = usuario.Apellido1,
                apellido2 = usuario.Apellido2 ?? string.Empty,
                correo = usuario.Correo,
                telefono = usuario.Telefono,
                rol = usuario.RolUsuarioNavigation?.Nombre ?? "-",
                fechaCreacion = usuario.FechaCreacion.ToString("dd/MM/yyyy")
            });
        }
    }
}
