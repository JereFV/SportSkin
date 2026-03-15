using Libreria.Web.Models;
using Microsoft.AspNetCore.Mvc;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Web.Models;
using SportSkin.Web.ViewModels;
using System.Diagnostics;
using System.Text.Json;

namespace SportSkin.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceSubasta _serviceSubasta;
        private readonly IServiceCamiseta _serviceCamiseta;

        public HomeController(
            ILogger<HomeController> logger,
            IServiceSubasta serviceSubasta,
            IServiceCamiseta serviceCamiseta)
        {
            _logger = logger;
            _serviceSubasta = serviceSubasta;
            _serviceCamiseta = serviceCamiseta;
        }

        public async Task<IActionResult> HomeIndex()
        {
            _logger.LogInformation("Entrando al método Index del HomeController");

            //Estable el usuario como una variable de sesión estática.
            var usuarioSesion = new { IdUsuario = 2, Nombre = "Rodrigo", Apellido1 = "Herrera", Apellido2 = "Castillo"};
            HttpContext.Session.SetString("UsuarioSesion", JsonSerializer.Serialize(usuarioSesion));

            var populares = await _serviceSubasta.GetSubastasMasPopularesAsync(3);
            var activas = await _serviceSubasta.GetSubastasActivasAsync(null, null);
            var camisetas = await _serviceCamiseta.ListAsync();

            var vm = new HomeViewModel
            {
                SubastasMasPopulares = populares,
                TotalSubastasActivas = activas.Count,
                TotalCamisetas = camisetas.Count
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ============================================================
        //            MÉTODO MANEJO DE ERRORES
        // ============================================================
        [HttpGet]
        public IActionResult ErrorHandler(string? messagesJson)
        {
            if (string.IsNullOrWhiteSpace(messagesJson))
            {
                ViewBag.ErrorMessages = new ErrorMiddlewareViewModel
                {
                    IdEvent = "SIN-DATO",
                    ListMessages = new List<string> { "No se recibió información de error." },
                    Path = "N/A"
                };
                return View("ErrorHandler");
            }

            ErrorMiddlewareViewModel? errorObject = null;
            try
            {
                errorObject = JsonSerializer.Deserialize<ErrorMiddlewareViewModel>(
                    messagesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al deserializar mensaje del middleware: {ex.Message}");
                errorObject = new ErrorMiddlewareViewModel
                {
                    IdEvent = "JSON-INVALIDO",
                    ListMessages = new List<string> { "El mensaje recibido no tiene un formato válido." }
                };
            }

            ViewBag.ErrorMessages = errorObject;
            return View("ErrorHandler");
        }
    }
}