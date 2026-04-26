using Libreria.Web.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Models;
using SportSkin.Web.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportSkin.Web.Controllers
{
    public class LoginController : Controller
    {
        IServiceUsuario _serviceUsuario;

        public LoginController(IServiceUsuario serviceUsuario)
        {
            _serviceUsuario = serviceUsuario;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([FromBody]LoginViewModel login)
        {
            try
            {
                var usuario = await _serviceUsuario.LoginAsync(login.Usuario, login.Clave);

                if (usuario == null)
                    return Json(new
                    {
                        statusCode = "warning",
                        message = "Las credenciales ingresadas no corresponden a ningún usuario registrado. Por favor intente nuevamente."
                    });

                if (!usuario.Estado)
                    return Json(new
                    {
                        statusCode = "warning",
                        message = "El usuario con el que se intenta acceder se encuentra deshabilitado. Por favor intente iniciar sesión con otro usuario."
                    });

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido1} {usuario.Apellido2}"),
                    new Claim(ClaimTypes.Role, usuario.RolUsuarioNavigation.Nombre),
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = false
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                );

                return Json(new
                {
                    statusCode = "success",
                    message = $"Inicio de sesión satisfactorio. Bienvenido(a) {usuario.Nombre}"
                });
            }
            catch
            {
                return Json(new
                {
                    statusCode = "error",
                    message = "Ha ocurrido un error inesperado al intentar realizar el inicio de sesión. Por favor intente de nuevo más tarde."
                });
            }
        }

        public async Task<IActionResult> LogOut()
        {
            try
            {
                await HttpContext.SignOutAsync();
            }
            catch (Exception)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error",
                    "Ha ocurrido un error al intentar cerrar la sesión. Por favor intente nuevamente en unos minutos.",
                    SweetAlertMessageType.success
                );
            }
                  
            return RedirectToAction("HomeIndex", "Home");
        }

        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
