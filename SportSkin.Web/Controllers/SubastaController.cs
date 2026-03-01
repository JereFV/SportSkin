using Microsoft.AspNetCore.Mvc;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Web.ViewModels;

namespace SportSkin.Web.Controllers
{
    public class SubastaController : Controller
    {
        private readonly IServiceSubasta _service;

        public SubastaController(IServiceSubasta service)
        {
            _service = service;
        }

        // GET: Subasta/Index
        public async Task<IActionResult> Index(string? filtro, DateTime? desde, DateTime? hasta)
        {
            ICollection<SubastaDTO> lista = filtro switch
            {
                "activas" => await _service.GetSubastasActivasAsync(desde, hasta),
                "finalizadas" => await _service.GetSubastasFinalizadasAsync(desde, hasta),
                "vendidas" => await _service.GetSubastasVendidasAsync(),
                _ => await _service.ListAsync()
            };

            ViewBag.FiltroActual = filtro;
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");

            return View(lista);
        }

        // GET: Subasta/MisSubastas
        // Por ahora con ID hardcodeado hasta que haya login
        public async Task<IActionResult> MisSubastas()
        {
            int idVendedor = 1; // cambiar cuando haya sesión
            var lista = await _service.GetSubastasByVendedorAsync(idVendedor);
            return View(lista);
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
        public async Task<IActionResult> Detalle(int id)
        {
            var subasta = await _service.FindByIdAsync(id);

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
    }
}
