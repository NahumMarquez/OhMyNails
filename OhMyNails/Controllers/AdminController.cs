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
                Id = c.Id,
                Nombre = c.Nombre,
                Telefono = c.Telefono,
                Fecha = c.Fecha,
                Hora = c.Hora,
                Categoria = c.Categoria,
                ImagenReferencia = c.ImagenReferencia,
                 Estado = c.Estado
            }).ToList();

            return View(viewModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarCita(int id)
        {
            var cita = _context.Citas.FirstOrDefault(c => c.Id == id);
            if (cita == null)
            {
                TempData["Mensaje"] = "❌ La cita no existe o ya fue cancelada.";
                return RedirectToAction("Citas");
            }

            // Marcar cancelada en BD
            cita.Estado = "Cancelada";
            _context.Citas.Update(cita);
            _context.SaveChanges();

            // Preparar y limpiar número para wa.me
            var mensaje = $"Hola {cita.Nombre}, tu cita del {cita.Fecha:dd/MM/yyyy} a las {cita.Hora} ha sido cancelada. 💅 - Oh My Nails";
            var numeroLimpio = cita.Telefono?.Replace(" ", "").Replace("-", "").Replace("+", "") ?? "";
            if (!numeroLimpio.StartsWith("503")) // si no trae código, agrega 503 (El Salvador)
            {
                numeroLimpio = "503" + numeroLimpio;
            }
            var urlWhatsApp = $"https://wa.me/{numeroLimpio}?text={Uri.EscapeDataString(mensaje)}";

            TempData["WhatsAppUrl"] = urlWhatsApp;
            TempData["Mensaje"] = $"La cita de {cita.Nombre} fue cancelada correctamente.";

            return RedirectToAction("Citas");
        }

    }
}
