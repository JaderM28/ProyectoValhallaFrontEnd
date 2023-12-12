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
    public class ClientesController : Controller
    {
        private readonly ValhallaDbContext _context;
        private readonly IConfiguration Configuration;
        private readonly IConverter _converter;

        public ClientesController(ValhallaDbContext context, IConfiguration configuration, IConverter converter)
        {
            _context = context;
            Configuration = configuration;
            _converter = converter;
        }

        // Excel reporte
        public IActionResult Exportar_excel()
        {
            DataTable tablaClientes = new DataTable();

            string connectionString = Configuration.GetConnectionString("Conexion");

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                using (var adapter = new SqlDataAdapter())
                {
                    adapter.SelectCommand = new SqlCommand("sp_report_Clientes", conexion);
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.Fill(tablaClientes);
                }
            }
            using (var libro = new XLWorkbook())
            {
                tablaClientes.TableName = "Clientes";
                var hoja = libro.Worksheets.Add(tablaClientes);
                hoja.ColumnsUsed().AdjustToContents();
                using (var memoria = new MemoryStream())
                {
                    libro.SaveAs(memoria);

                    var nombreExcel = string.Concat("Reporte Cliente", DateTime.Now.ToString(), ".xlsx");
                    return File(memoria.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreExcel);
                }
            }
        }

        // GET: Clientes
        public async Task<IActionResult> Index(string buscar, string filtro, int? pagina)
        {

            int tamañoPagina = 5;
            int numeroPagina = pagina ?? 1;

            var busClientes = from Cliente in _context.Clientes select Cliente;
            string NombreAsc = "NombreAscendente";
            string NombreDes = "NombreDescendente";

            if (!String.IsNullOrEmpty(buscar))
            {
                busClientes = busClientes.Where(Cliente => Cliente.Nombres!.Contains(buscar));
            }

            ViewData["FiltroNombre"] = filtro == NombreAsc ? NombreDes : NombreAsc;

            if (filtro == NombreDes)
            {
                busClientes = busClientes.OrderByDescending(s => s.Nombres);
            }
            else if (filtro == NombreAsc)
            {
                busClientes = busClientes.OrderBy(s => s.Nombres);
            }

            return View(await busClientes.ToPagedListAsync(numeroPagina, tamañoPagina));

        }

        //PDF
        private List<Cliente> ObtenerDatos()
        {
            var clientes = _context.Clientes.
                Select(u => new Cliente
                {
                    IdCliente = u.IdCliente,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Telefono = u.Telefono,
                    TipoDocumento = u.TipoDocumento,
                    NumeroDocumento = u.NumeroDocumento,
                    Direccion = u.Direccion,
                    Genero = u.Genero,
                    FechaNacimiento = u.FechaNacimiento
                }).ToList();
            return clientes;
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
            url_pagina = $"{url_pagina}/Clientes/VistaParaPDF";

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
            string nombrePDF = "ReporteClientes" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
            return File(archivoPDF, "application/pdf", nombrePDF);
        }
        //CIERRE DE PDF


        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,Nombres,Apellidos,Telefono,TipoDocumento,NumeroDocumento,Direccion,Genero,FechaNacimiento")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,Nombres,Apellidos,Telefono,TipoDocumento,NumeroDocumento,Direccion,Genero,FechaNacimiento")] Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.IdCliente))
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
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clientes == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.Clientes'  is null.");
            }
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
          return (_context.Clientes?.Any(e => e.IdCliente == id)).GetValueOrDefault();
        }
    }
}
