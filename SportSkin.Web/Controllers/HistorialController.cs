//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SportSkin.Services;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace SportSkin.Controllers
//{
//    [Authorize]
//    public class HistorialController : Controller
//    {
//        private readonly IHistorialService _service;

//        public HistorialController(IHistorialService service)
//        {
//            _service = service;
//        }

//        // GET /Historial/GetHistorial
//        // Llamado via fetch() desde el menú. Devuelve el partial con los datos.
//        [HttpGet]
//        public async Task<IActionResult> GetHistorial()
//        {
//            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
//            var rol       = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

//            if (rol == "Comprador")
//            {
//                var vm = await _service.GetHistorialCompradorAsync(idUsuario);
//                return PartialView("_HistorialContenido", vm);
//            }

//            if (rol == "Vendedor")
//            {
//                var vm = await _service.GetHistorialVendedorAsync(idUsuario);
//                return PartialView("_HistorialContenido", vm);
//            }

//            return Unauthorized();
//        }
//    }
//}
