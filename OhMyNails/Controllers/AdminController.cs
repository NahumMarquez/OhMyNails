using Microsoft.AspNetCore.Mvc;
using OhMyNails.Data;
using OhMyNails.Models;
using OhMyNails.Services;


namespace OhMyNails.Controllers
{
    public class AdminController : Controller
    {
        private readonly CitaService _citaService;
        private readonly ApplicationDbContext _context;

        public AdminController(CitaService citaService, ApplicationDbContext context)
        {
            _citaService = citaService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Citas()
        {
            var usuario = HttpContext.Session.GetString("Rol");
            if (usuario != "Admin")
            {
                return RedirectToAction("Login", "Access");
            }

            var citas = _citaService.ObtenerCitas();
            var viewModels = citas.Select(c => new CitaViewModel
            {
                Nombre = c.Nombre,
                Telefono = c.Telefono,
                Fecha = c.Fecha,
                Hora = c.Hora,
                Categoria = c.Categoria
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Servicios()
        {
            var usuario = HttpContext.Session.GetString("Rol");
            if (usuario != "Admin")
            {
                return RedirectToAction("Login", "Access");
            }

            var servicios = _context.Servicios.ToList();
            return View(servicios);
        }


        [HttpPost]
        public IActionResult EditarServicio(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Servicios.Update(servicio);
                _context.SaveChanges();
                TempData["Mensaje"] = "Servicio actualizado correctamente.";
            }
            return RedirectToAction("Servicios");
        }
    }
}
