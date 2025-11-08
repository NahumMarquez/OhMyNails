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
                Categoria = c.Categoria,
                ImagenReferencia = c.ImagenReferencia
            }).ToList();

            return View(viewModels);
        }

        [HttpPost]
        public IActionResult CancelarCita(int id)
        {
            var cita = _context.Citas.FirstOrDefault(c => c.Id == id);
            if (cita == null)
            {
                TempData["Mensaje"] = "❌ La cita no existe o ya fue cancelada.";
                return RedirectToAction("Citas");
            }

            cita.Estado = "Cancelada";
            _context.Citas.Update(cita);
            _context.SaveChanges();

            // ✅ Enviar mensaje por WhatsApp (enlace)
            var mensaje = $"Hola {cita.Nombre}, tu cita del {cita.Fecha:dd/MM/yyyy} a las {cita.Hora} ha sido cancelada. 💅 - Oh My Nails";
            var urlWhatsApp = $"https://wa.me/{cita.Telefono.Replace(" ", "").Replace("+", "")}?text={Uri.EscapeDataString(mensaje)}";

            TempData["WhatsAppUrl"] = urlWhatsApp;
            TempData["Mensaje"] = "La cita fue cancelada correctamente.";

            return RedirectToAction("Citas");
        }
    }
}
