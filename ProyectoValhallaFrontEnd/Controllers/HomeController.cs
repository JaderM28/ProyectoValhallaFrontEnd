using Microsoft.AspNetCore.Mvc;
using ProyectoValhallaFrontEnd.Models;
using ProyectoValhallaFrontEnd.Models;
using System.Diagnostics;

namespace ProyectoValhallaFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var ObjPersona = new InformacionPersonal()
            {
                Nombre = "Somos BelcomSpa",
                Estudio = "Centro de Belleza Personal y Barberia",
                URL = "https://www.instagram.com/belcomspa/?hl=es-la",

            };

            var modelo = new HomeIndex() { DatosPersona = ObjPersona };

            return View(modelo);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}