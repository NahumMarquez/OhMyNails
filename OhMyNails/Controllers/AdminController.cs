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

            var citas = _citaService.ObtenerCitas()
           .OrderBy(c => c.Fecha)
           .ThenBy(c => TimeSpan.Parse(c.Hora))
           .ToList();
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

            // 🔹 Marcar cancelada
            cita.Estado = "Cancelada";
            _context.Citas.Update(cita);
            _context.SaveChanges();

            // 🔹 Generar enlace de WhatsApp
            var mensaje = $"Hola {cita.Nombre}, tu cita del {cita.Fecha:dd/MM/yyyy} a las {cita.Hora} ha sido cancelada. 💅 - Oh My Nails";
            var numeroLimpio = cita.Telefono?.Replace(" ", "").Replace("-", "").Replace("+", "") ?? "";
            if (!numeroLimpio.StartsWith("503"))
                numeroLimpio = "503" + numeroLimpio;

            var urlWhatsApp = $"https://wa.me/{numeroLimpio}?text={Uri.EscapeDataString(mensaje)}";

            // ✅ En lugar de solo redirigir, abrimos WhatsApp directamente
            return Redirect(urlWhatsApp);
        }

        // ✅ NUEVA ACCIÓN: Eliminar Citas Pasadas o Canceladas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCita(int id)
        {
            var cita = _context.Citas.FirstOrDefault(c => c.Id == id);
            if (cita == null)
            {
                TempData["Mensaje"] = "⚠️ La cita ya no existe.";
                return RedirectToAction("Citas");
            }

            // 🔹 Verifica si la cita ya pasó o está cancelada
            bool citaPasada = cita.Fecha < DateTime.Today ||
                              (cita.Fecha == DateTime.Today && TimeSpan.Parse(cita.Hora) < DateTime.Now.TimeOfDay);

            bool citaCancelada = cita.Estado?.Equals("Cancelada", StringComparison.OrdinalIgnoreCase) == true;

            if (!citaPasada && !citaCancelada)
            {
                TempData["Mensaje"] = "⛔ Solo se pueden eliminar citas que ya hayan pasado o que estén canceladas.";
                return RedirectToAction("Citas");
            }

            // 🔹 Eliminar cita
            _context.Citas.Remove(cita);
            _context.SaveChanges();

            TempData["Mensaje"] = $"🗑️ La cita de {cita.Nombre} fue eliminada correctamente.";
            return RedirectToAction("Citas");
        }
    }
}
