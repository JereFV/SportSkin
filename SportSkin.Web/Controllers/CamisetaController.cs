using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Web.ViewModels;
using System.Globalization;

namespace SportSkin.Web.Controllers
{
    public class CamisetaController : Controller
    {
        private readonly IServiceCamiseta _serviceCamiseta;
        private readonly IServiceCategoriaCamiseta _serviceCategoriaCamiseta;
        private readonly IServiceCondicionCamiseta _serviceCondicionCamiseta;

        public CamisetaController(IServiceCamiseta service, IServiceCategoriaCamiseta serviceCategoriaCamiseta, IServiceCondicionCamiseta serviceCondicionCamiseta)
        {
            _serviceCamiseta = service;
            _serviceCategoriaCamiseta = serviceCategoriaCamiseta;
            _serviceCondicionCamiseta = serviceCondicionCamiseta;
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

        // GET: Camiseta/Create
        public IActionResult CamisetaCreate()
        {
            return View();
        }

        // POST: Camiseta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CamisetaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _serviceCamiseta.AddAsync(dto);
                TempData["success"] = "Camiseta creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
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
    }
}
