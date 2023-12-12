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
using Microsoft.AspNetCore.Authorization;

namespace ValhallaProyecto.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly ValhallaDbContext _context;
        private readonly IConfiguration Configuration;
        private readonly IConverter _converter;

        public RolesController(ValhallaDbContext context, IConfiguration configuration, IConverter converter)
        {
            _context = context;
            Configuration = configuration;
            _converter = converter;
        }

        // Excel reporte
        public IActionResult Exportar_excel()
        {
            DataTable tablaUsuarios = new DataTable();

            string connectionString = Configuration.GetConnectionString("Conexion");

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                using (var adapter = new SqlDataAdapter())
                {
                    adapter.SelectCommand = new SqlCommand("sp_report_Roles", conexion);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.Fill(tablaUsuarios);
                }
            }
            using (var libro = new XLWorkbook())
            {
                tablaUsuarios.TableName = "Roles";
                var hoja = libro.Worksheets.Add(tablaUsuarios);
                hoja.ColumnsUsed().AdjustToContents();
                using (var memoria = new MemoryStream())
                {
                    libro.SaveAs(memoria);

                    var nombreExcel = string.Concat("Reporte Roles", DateTime.Now.ToString(), ".xlsx");
                    return File(memoria.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreExcel);
                }
            }
        }

        //PDF
        private List<Role> ObtenerDatos()
        {
            var roles = _context.Roles.
                Select(u => new Role
                {
                    IdRol = u.IdRol,
                    NombreRol = u.NombreRol,
                    Descripcion = u.Descripcion,  
                }).ToList();
            return roles;
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
            url_pagina = $"{url_pagina}/Roles/VistaParaPDF";

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
            string nombrePDF = "ReporteRoles" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
            return File(archivoPDF, "application/pdf", nombrePDF);
        }
        //CIERRE DE PDF

        // GET: Roles
        public async Task<IActionResult> Index(string buscar, string filtro, int? pagina)
        {
            int tamañoPagina = 5;
            int numeroPagina = pagina ?? 1;

            var busRoles = from Role in _context.Roles select Role;
            string NombreAsc = "NombreAscendente";
            string NombreDes = "NombreDescendente";

            if (!String.IsNullOrEmpty(buscar))
            {
                busRoles = busRoles.Where(Role => Role.NombreRol!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = filtro == NombreAsc ? NombreDes : NombreAsc;

            if (filtro == NombreDes)
            {
                busRoles = busRoles.OrderByDescending(s => s.NombreRol);
            }
            else if (filtro == NombreAsc)
            {
                busRoles = busRoles.OrderBy(s => s.NombreRol);
            }

            return View(await busRoles.ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRol,Nombre,Descripcion")] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,Nombre,Descripcion")] Role role)
        {
            if (id != role.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.IdRol))
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
            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
          return (_context.Roles?.Any(e => e.IdRol == id)).GetValueOrDefault();
        }
    }
}
