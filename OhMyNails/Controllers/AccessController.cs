using Microsoft.AspNetCore.Mvc;

namespace OhMyNails.Controllers
{
    public class AccessController : Controller
    {
        private const string ClaveAdmin = "OhMyNails2025"; // 🔐 tu clave secreta

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string clave)
        {
            if (clave == ClaveAdmin)
            {
                HttpContext.Session.SetString("Rol", "Admin");
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Clave incorrecta. Intenta de nuevo.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
