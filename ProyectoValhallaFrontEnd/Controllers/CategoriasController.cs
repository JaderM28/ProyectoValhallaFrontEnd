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
//using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using System.Configuration;

namespace ValhallaProyecto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ValhallaDbContext _context;
        private readonly IConfiguration Configuration;
        private readonly IConverter _converter;

        public CategoriasController(ValhallaDbContext context, IConfiguration configuration, IConverter converter)
        {
           
            _context = context;
            Configuration = configuration;
            _converter = converter;
        }

        // Excel reporte
        public IActionResult Exportar_excel()
        {
            DataTable tablaCategorias = new DataTable();

            string connectionString = Configuration.GetConnectionString("Conexion");

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                using (var adapter = new SqlDataAdapter())
                {
                    adapter.SelectCommand = new SqlCommand("sp_report_Categorias", conexion);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.Fill(tablaCategorias);
                }
            }
            using (var libro = new XLWorkbook())
            {
                tablaCategorias.TableName = "Categorias";
                var hoja = libro.Worksheets.Add(tablaCategorias);
                hoja.ColumnsUsed().AdjustToContents();
                using (var memoria = new MemoryStream())
                {
                    libro.SaveAs(memoria);

                    var nombreExcel = string.Concat("Reporte Categorias", DateTime.Now.ToString(), ".xlsx");
                    return File(memoria.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreExcel);
                }
            }
        }

        // GET: Categorias
        public async Task<IActionResult> Index(string buscar, string filtro, int? pagina)
        {
            int tamañoPagina = 5;
            int numeroPagina = pagina ?? 1;

            var busCategorias = from Categoria in _context.Categorias select Categoria;
            string NombreAsc = "NombreAscendente";
            string NombreDes = "NombreDescendente";

            if (!String.IsNullOrEmpty(buscar))
            {
                busCategorias = busCategorias.Where(Categoria => Categoria.Nombre!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = filtro == NombreAsc ? NombreDes : NombreAsc;

            if (filtro == NombreDes)
            {
                busCategorias = busCategorias.OrderByDescending(s => s.Nombre);
            }
            else if (filtro == NombreAsc)
            {
                busCategorias = busCategorias.OrderBy(s => s.Nombre);
            }

            return View(await busCategorias.ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        //PDF
        private List<Categoria> ObtenerDatos()
        {
            var categorias = _context.Categorias.
                Select(u => new Categoria
                {
                    IdCategoria = u.IdCategoria,
                    Nombre = u.Nombre,
                    Descripcion = u.Descripcion,
                    Observaciones = u.Observaciones,                 
                }).ToList();
            return categorias;
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
            url_pagina = $"{url_pagina}/Categorias/VistaParaPDF";

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
            string nombrePDF = "ReporteCategorias" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
            return File(archivoPDF, "application/pdf", nombrePDF);
        }
        //CIERRE DE PDF



        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategoria,Nombre,Descripcion,Observaciones")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoria,Nombre,Descripcion,Observaciones")] Categoria categoria)
        {
            if (id != categoria.IdCategoria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.IdCategoria))
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
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categorias == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.Categorias'  is null.");
            }
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
          return (_context.Categorias?.Any(e => e.IdCategoria == id)).GetValueOrDefault();
        }
    }
}
