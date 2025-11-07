using Microsoft.AspNetCore.Mvc;
using OhMyNails.Models;
using System.Diagnostics;

namespace OhMyNails.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var categorias = new List<(string Nombre, string Imagen)>
            {
                ("Softgel", "/css/Imagenes/softgel1.jpeg"),
                ("Rubber Gel", "/css/Imagenes/rubbergel1.jpeg"),
                ("Builder Gel", "/css/Imagenes/buildergel1.jpeg")
            };

            return View(categorias);
        }

        // Páginas de categorías
        public IActionResult Softgel()
        {
            return View();
        }

        public IActionResult RubberGel()
        {
            return View();
        }

        public IActionResult BuilderGel()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
