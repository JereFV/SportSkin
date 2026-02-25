using Microsoft.AspNetCore.Mvc;
using SportSkin.Application.Services.Interfaces;

namespace SportSkin.Web.Controllers
{
    public class SubastaController : Controller
    {
        private readonly IServiceSubasta _service;

        public SubastaController(IServiceSubasta service)
        {
            _service = service;
        }

        // GET: Subasta/Activas?desde=2024-01-01&hasta=2024-12-31
        public async Task<IActionResult> Activas(DateTime? desde, DateTime? hasta)
        {
            var lista = await _service.GetSubastasActivasAsync(desde, hasta);
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            return View(lista);
        }

        // GET: Subasta/Finalizadas?desde=2024-01-01&hasta=2024-12-31
        public async Task<IActionResult> Finalizadas(DateTime? desde, DateTime? hasta)
        {
            var lista = await _service.GetSubastasFinalizadasAsync(desde, hasta);
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            return View(lista);
        }

        // GET: Subasta/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var subasta = await _service.FindByIdAsync(id);
            if (subasta == null)
                return NotFound();

            return View(subasta);
        }
    }
}
