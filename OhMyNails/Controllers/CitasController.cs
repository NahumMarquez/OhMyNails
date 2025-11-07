using Microsoft.AspNetCore.Mvc;
using OhMyNails.Data;
using OhMyNails.Models;
using OhMyNails.Services;
using System.Linq;

namespace OhMyNails.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly CitaService _citaService;

        // 🔹 Inyectamos ambos: la base de datos y el servicio
        public CitasController(ApplicationDbContext context, CitaService citaService)
        {
            _context = context;
            _citaService = citaService;
        }

        public IActionResult Agendar()
        {
            var model = new CitaViewModel
            {
                Fecha = DateTime.Today
            };

            // 🔹 Si el usuario es admin, mostramos las citas existentes
            if (HttpContext.Session.GetString("Rol") == "Admin")
            {
                ViewBag.Citas = _context.Citas.ToList();
            }

            return View(new CitaViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Agendar(CitaViewModel model)
        {
            Console.WriteLine("📩 POST Agendar invoked");

            // logear contenidos recibidos (útil para debug)
            Console.WriteLine($"Nombre='{model?.Nombre}', Telefono='{model?.Telefono}', Fecha='{model?.Fecha}', Hora='{model?.Hora}', Categoria='{model?.Categoria}'");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState inválido:");
                foreach (var entry in ModelState)
                {
                    foreach (var err in entry.Value.Errors)
                    {
                        Console.WriteLine($" - {entry.Key}: {err.ErrorMessage}");
                    }
                }

                // Si no es válido, recargamos las citas para admin y devolvemos la vista con model y errores
                if (HttpContext.Session.GetString("Rol") == "Admin")
                {
                    ViewBag.Citas = _context.Citas.ToList();
                }
                return View(model);
            }

            // Guardar imagen si viene
            string imagenRuta = null;
            var file = Request.Form.Files.FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                imagenRuta = "/uploads/" + fileName;
            }

            var cita = new Cita
            {
                Nombre = model.Nombre,
                Telefono = model.Telefono,
                Fecha = model.Fecha,
                Hora = model.Hora,
                Categoria = model.Categoria,
                
            };

            _citaService.AgregarCita(cita);

            string mensaje = $"💅 *Cita Agendada*\n" +
                     $"👤 Cliente: {cita.Nombre}\n" +
                     $"📞 Teléfono: {cita.Telefono}\n" +
                     $"💅 Servicio: {cita.Categoria}\n" +
                     $"🗓 Fecha: {cita.Fecha:dd/MM/yyyy}\n" +
                     $"⏰ Hora: {cita.Hora}";

            // Número de tu negocio
            string numeroNegocio = "50379888479";

            // Crear enlace de WhatsApp
            string whatsappUrl = $"https://wa.me/{numeroNegocio}?text={Uri.EscapeDataString(mensaje)}";

            // Guardamos el enlace temporalmente
            TempData["WhatsappUrl"] = whatsappUrl;

            return RedirectToAction("Confirmacion", new { id = cita.Id, url = whatsappUrl });
        }
        


        public IActionResult Confirmacion(int id)
        {
            var cita = _context.Citas.FirstOrDefault(c => c.Id == id);
            if (cita == null)
                return RedirectToAction("Agendar");

            var model = new CitaViewModel
            {
                Nombre = cita.Nombre,
                Telefono = cita.Telefono,
                Fecha = cita.Fecha,
                Hora = cita.Hora,
                Categoria = cita.Categoria,
                ImagenReferencia = cita.Imagen
            };

            string mensaje = $"Hola {model.Nombre}! Tu cita en *Oh my Nails* fue confirmada para el {model.Fecha:dd/MM/yyyy} a las {model.Hora} ({model.Categoria}). 💅";
            string whatsappUrl = $"https://wa.me/50379888479?text={Uri.EscapeDataString(mensaje)}";

            ViewBag.WhatsappUrl = whatsappUrl;
            ViewBag.Mensaje = mensaje;

            return View(model);
        }


        [HttpGet]
        public IActionResult ObtenerHorarios(DateTime fecha)
        {
            var horarios = _citaService.ObtenerHorarios(fecha);
            var lista = horarios.Select(h => new {
                hora = h,
                disponible = !_citaService.EstaOcupada(fecha, h)
            });
            return Json(lista);

        }

    }
}
