using Libreria.Web.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportSkin.Application.Services.Interfaces;
using System.Threading.Tasks;

namespace SportSkin.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IServiceUsuario _service;

        public UsuarioController(IServiceUsuario service)
        {
            _service = service;
        }

        // GET: UsuarioController
        public async Task<ActionResult> Index()
        {
            try
            {
                var usuarios = await _service.ListAsync();

                return View(usuarios);
            }
            catch (Exception ex) 
            {
                ViewBag.Exception = SweetAlertHelper.CrearNotificacion("Listado de usuarios", "Ha ocurrido un error al intentar obtener el listado de usuarios.", SweetAlertMessageType.error);
                throw;
            }        
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
