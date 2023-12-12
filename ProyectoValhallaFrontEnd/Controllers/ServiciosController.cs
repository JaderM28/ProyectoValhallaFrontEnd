using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoValhallaFrontEnd.Models;
using X.PagedList;
using System.Data;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace ValhallaProyecto.Controllers
{
    [Authorize]
    public class ServiciosController : Controller
    {
        private readonly ValhallaDbContext _context;
        private readonly IConfiguration Configuration;
        private readonly IConverter _converter;

        public ServiciosController(ValhallaDbContext context, IConfiguration configuration, IConverter converter)
        {
            _context = context;
            Configuration = configuration;
            _converter = converter;
        }

        // Excel reporte
        public IActionResult Exportar_excel()
        {
            DataTable tablaServicios = new DataTable();

            string connectionString = Configuration.GetConnectionString("Conexion");

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                using (var adapter = new SqlDataAdapter())
                {
                    adapter.SelectCommand = new SqlCommand("sp_report_Servicios", conexion);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.Fill(tablaServicios);
                }
            }
            using (var libro = new XLWorkbook())
            {
                tablaServicios.TableName = "Servicios";
                var hoja = libro.Worksheets.Add(tablaServicios);
                hoja.ColumnsUsed().AdjustToContents();
                using (var memoria = new MemoryStream())
                {
                    libro.SaveAs(memoria);

                    var nombreExcel = string.Concat("Reporte Servicios", DateTime.Now.ToString(), ".xlsx");
                    return File(memoria.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreExcel);
                }
            }
        }


        // GET: Servicios
        public async Task<IActionResult> Index(string buscar, string filtro, int? pagina)
        {
            int tamañoPagina = 5;
            int numeroPagina = pagina ?? 1;

            var busServicios = from Servicio in _context.Servicios.Include(l => l.IdCategoriaNavigation) select Servicio;
            string NombreAsc = "NombreAscendente";
            string NombreDes = "NombreDescendente";

            if (!String.IsNullOrEmpty(buscar))
            {
                busServicios = busServicios.Where(Servicio => Servicio.Nombre!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = filtro == NombreAsc ? NombreDes : NombreAsc;

            if (filtro == NombreDes)
            {
                busServicios = busServicios.OrderByDescending(s => s.Nombre);
            }
            else if (filtro == NombreAsc)
            {
                busServicios = busServicios.OrderBy(s => s.Nombre);
            }

            return View(await busServicios.ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        //PDF
        private List<Servicio> ObtenerDatos()
        {

            var servicios = _context.Servicios.Include(u => u.IdCategoriaNavigation).
                Select(u => new Servicio
                {
                    IdServicio = u.IdServicio,
                    Nombre = u.Nombre,
                    Precio = u.Precio,
                    DuracionAproximada = u.DuracionAproximada,
                    Descripcion = u.Descripcion,
                    IdCategoriaNavigation = u.IdCategoriaNavigation 
                }).ToList();

            return servicios;
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
            url_pagina = $"{url_pagina}/Servicios/VistaParaPDF";

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
            string nombrePDF = "ReporteServicios" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
            return File(archivoPDF, "application/pdf", nombrePDF);
        }
        //CIERRE DE PDF

        // GET: Servicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Servicios == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .Include(s => s.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdServicio == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create()
        {
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "IdCategoria");
            return View();
        }

        // POST: Servicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdServicio,Nombre,Precio,DuracionAproximada,Descripcion,IdCategoria")] Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "IdCategoria", servicio.IdCategoria);
            return View(servicio);
        }

        // GET: Servicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Servicios == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "IdCategoria", servicio.IdCategoria);
            return View(servicio);
        }

        // POST: Servicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdServicio,Nombre,Precio,DuracionAproximada,Descripcion,IdCategoria")] Servicio servicio)
        {
            if (id != servicio.IdServicio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioExists(servicio.IdServicio))
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
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "IdCategoria", servicio.IdCategoria);
            return View(servicio);
        }

        // GET: Servicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Servicios == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .Include(s => s.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdServicio == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // POST: Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Servicios == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.Servicios'  is null.");
            }
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicioExists(int id)
        {
          return (_context.Servicios?.Any(e => e.IdServicio == id)).GetValueOrDefault();
        }
    }
}
