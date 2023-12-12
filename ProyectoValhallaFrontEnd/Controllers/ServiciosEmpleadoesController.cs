using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoValhallaFrontEnd.Models;

namespace ValhallaProyecto.Controllers
{
    [Authorize]
    public class ServiciosEmpleadoesController : Controller
    {
        private readonly ValhallaDbContext _context;

        public ServiciosEmpleadoesController(ValhallaDbContext context)
        {
            _context = context;
        }

        // GET: ServiciosEmpleadoes
        public async Task<IActionResult> Index()
        {
            var valhallaProyectoContext = _context.ServiciosEmpleados.Include(s => s.IdEmpleadoNavigation).Include(s => s.IdServicioNavigation);
            return View(await valhallaProyectoContext.ToListAsync());
        }

        // GET: ServiciosEmpleadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ServiciosEmpleados == null)
            {
                return NotFound();
            }

            var serviciosEmpleado = await _context.ServiciosEmpleados
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdServicioEmpleado == id);
            if (serviciosEmpleado == null)
            {
                return NotFound();
            }

            return View(serviciosEmpleado);
        }

        // GET: ServiciosEmpleadoes/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado");
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio");
            return View();
        }

        // POST: ServiciosEmpleadoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdServicioEmpleado,IdEmpleado,IdServicio")] ServiciosEmpleado serviciosEmpleado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviciosEmpleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", serviciosEmpleado.IdEmpleado);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", serviciosEmpleado.IdServicio);
            return View(serviciosEmpleado);
        }

        // GET: ServiciosEmpleadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ServiciosEmpleados == null)
            {
                return NotFound();
            }

            var serviciosEmpleado = await _context.ServiciosEmpleados.FindAsync(id);
            if (serviciosEmpleado == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", serviciosEmpleado.IdEmpleado);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", serviciosEmpleado.IdServicio);
            return View(serviciosEmpleado);
        }

        // POST: ServiciosEmpleadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdServicioEmpleado,IdEmpleado,IdServicio")] ServiciosEmpleado serviciosEmpleado)
        {
            if (id != serviciosEmpleado.IdServicioEmpleado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviciosEmpleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiciosEmpleadoExists(serviciosEmpleado.IdServicioEmpleado))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", serviciosEmpleado.IdEmpleado);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", serviciosEmpleado.IdServicio);
            return View(serviciosEmpleado);
        }

        // GET: ServiciosEmpleadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ServiciosEmpleados == null)
            {
                return NotFound();
            }

            var serviciosEmpleado = await _context.ServiciosEmpleados
                .Include(s => s.IdEmpleadoNavigation)
                .Include(s => s.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdServicioEmpleado == id);
            if (serviciosEmpleado == null)
            {
                return NotFound();
            }

            return View(serviciosEmpleado);
        }

        // POST: ServiciosEmpleadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ServiciosEmpleados == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.ServiciosEmpleados'  is null.");
            }
            var serviciosEmpleado = await _context.ServiciosEmpleados.FindAsync(id);
            if (serviciosEmpleado != null)
            {
                _context.ServiciosEmpleados.Remove(serviciosEmpleado);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiciosEmpleadoExists(int id)
        {
          return (_context.ServiciosEmpleados?.Any(e => e.IdServicioEmpleado == id)).GetValueOrDefault();
        }
    }
}
