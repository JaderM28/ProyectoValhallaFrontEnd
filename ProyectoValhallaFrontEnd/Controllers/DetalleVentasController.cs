using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoValhallaFrontEnd.Models;
using System.Diagnostics;
using ProyectoValhallaFrontEnd.Models;

namespace ValhallaProyecto.Controllers
{
    public class DetalleVentasController : Controller
    {
        private readonly ValhallaDbContext _context;

        public DetalleVentasController(ValhallaDbContext context)
        {
            _context = context;
        }

        public IActionResult resumenVenta()
        {
            DateTime FechaInicio = DateTime.Now;
            FechaInicio = FechaInicio.AddDays(-5);

            List<VMVenta> Lista = (from tbventa in _context.Venta where tbventa.FechaRegistro.Value.Date >= FechaInicio.Date group tbventa by tbventa.FechaRegistro.Value.Date into grupo
                            select new VMVenta
                            {   
                                fecha = grupo.Key.ToString("dd/MM/yyyy"),
                                cantidad = grupo.Count(),
                            }).ToList();
            return StatusCode(StatusCodes.Status200OK, Lista);
        }
        public IActionResult resumenProducto()
        {
            List<VMProducto> Lista = (from tbdetalleventa in _context.DetalleVenta group tbdetalleventa by tbdetalleventa.NombreProducto into grupo orderby grupo.Count() descending select new VMProducto
                            {
                                producto = grupo.Key,
                                cantidad = grupo.Count(),
                            }).Take(4).ToList();
            return StatusCode(StatusCodes.Status200OK, Lista);
        }

        public IActionResult Index()
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
