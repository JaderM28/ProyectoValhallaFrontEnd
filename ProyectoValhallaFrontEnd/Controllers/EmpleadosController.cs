using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoValhallaFrontEnd.Models;
using X.PagedList;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using ProyectoValhallaFrontEnd.Models;
using Microsoft.AspNetCore.Authorization;

namespace ValhallaProyecto.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        private readonly ValhallaDbContext _context;
        private readonly IConfiguration Configuration;
        private readonly IConverter _converter;

        public EmpleadosController(ValhallaDbContext context,IConfiguration configuration, IConverter converter)
        {
            _context = context;
            Configuration = configuration;
            _converter = converter;
        }

        public IActionResult Exportar_excel()
        {
            DataTable tablaEmpleados = new DataTable();

            string connectionString = Configuration.GetConnectionString("Conexion");

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                using (var adapter = new SqlDataAdapter())
                {
                    adapter.SelectCommand = new SqlCommand("sp_report_Empleados", conexion);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.Fill(tablaEmpleados);
                }
            }
            using (var libro = new XLWorkbook())
            {
                tablaEmpleados.TableName = "Empleados";
                var hoja = libro.Worksheets.Add(tablaEmpleados);
                hoja.ColumnsUsed().AdjustToContents();
                using (var memoria = new MemoryStream())
                {
                    libro.SaveAs(memoria);

                    var nombreExcel = string.Concat("Reporte Empleado", DateTime.Now.ToString(), ".xlsx");
                    return File(memoria.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreExcel);
                }
            }
        }

        // GET: Empleados
        public async Task<IActionResult> Index(string buscar, string filtro, int? pagina)
        {

            int tamañoPagina = 5;
            int numeroPagina = pagina ?? 1;

            var busEmpleados = from Empleado in _context.Empleados.Include(l => l.IdServicioNavigation) select Empleado;
            string NombreAsc = "NombreAscendente";
            string NombreDes = "NombreDescendente";

            if (!String.IsNullOrEmpty(buscar))
            {
                busEmpleados = busEmpleados.Where(Empleado => Empleado.Nombres!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = filtro == NombreAsc ? NombreDes : NombreAsc;

            if (filtro == NombreDes)
            {
                busEmpleados = busEmpleados.OrderByDescending(s => s.Nombres);
            }
            else if (filtro == NombreAsc)
            {
                busEmpleados = busEmpleados.OrderBy(s => s.Nombres);
            }

            return View(await busEmpleados.ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        //PDF
        private List<Empleado> ObtenerDatos()
        {
            var empleados = _context.Empleados.Include(u => u.IdServicioNavigation).
                Select(u => new Empleado
                {
                    IdEmpleado = u.IdEmpleado,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Telefono = u.Telefono,
                    TipoDocumento = u.TipoDocumento,
                    NumeroDocumento = u.NumeroDocumento,
                    Direccion = u.Direccion,
                    Genero = u.Genero,
                    FechaNacimiento = u.FechaNacimiento,
                    IdServicioNavigation = u.IdServicioNavigation
                }).ToList();

            return empleados;
        }

        public IActionResult VistaParaPDF()
        {
            var model = ObtenerDatos();

            if (model == null)
            {
                // Manejar el caso donde ObtenerDatos() devuelve null, por ejemplo, redirigir a una página de error.
                return RedirectToAction("Error");
            }

            return View(model);
        }
        public IActionResult DescargarPDF()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Empleados/VistaParaPDF";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                new ObjectSettings()
                {
                    Page= url_pagina
                }
                }
            };
            var archivoPDF = _converter.Convert(pdf);
            string nombrePDF = "ReporteEmpleados" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
            return File(archivoPDF, "application/pdf", nombrePDF);
        }
        //CIERRE DE PDF


        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empleados == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio");
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleado,Nombres,Apellidos,Telefono,TipoDocumento,NumeroDocumento,Direccion,Genero,FechaNacimiento,IdServicio")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", empleado.IdServicio);
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Empleados == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", empleado.IdServicio);
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,Nombres,Apellidos,Telefono,TipoDocumento,NumeroDocumento,Direccion,Genero,FechaNacimiento,IdServicio")] Empleado empleado)
        {
            if (id != empleado.IdEmpleado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.IdEmpleado))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", empleado.IdServicio);
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empleados == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empleados == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.Empleados'  is null.");
            }
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
          return (_context.Empleados?.Any(e => e.IdEmpleado == id)).GetValueOrDefault();
        }
    }
}
