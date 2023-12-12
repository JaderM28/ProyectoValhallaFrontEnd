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
    public class AgendaController : Controller
    {
        private readonly ValhallaDbContext _context;

        public AgendaController(ValhallaDbContext context)
        {
            _context = context;
        }

        // GET: Agenda
        public async Task<IActionResult> Index()
        {
            var valhallaProyectoContext = _context.Agendas.Include(a => a.IdEmpleadoNavigation);
            return View(await valhallaProyectoContext.ToListAsync());
        }

        // GET: Agenda/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Agendas == null)
            {
                return NotFound();
            }

            var agenda = await _context.Agendas
                .Include(a => a.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdAgenda == id);
            if (agenda == null)
            {
                return NotFound();
            }

            return View(agenda);
        }

        // GET: Agenda/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado");
            return View();
        }

        // POST: Agenda/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAgenda,HoraInicio,HoraFin,Fecha,Estado,IdEmpleado")] Agenda agenda)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agenda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", agenda.IdEmpleado);
            return View(agenda);
        }

        // GET: Agenda/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Agendas == null)
            {
                return NotFound();
            }

            var agenda = await _context.Agendas.FindAsync(id);
            if (agenda == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", agenda.IdEmpleado);
            return View(agenda);
        }

        // POST: Agenda/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAgenda,HoraInicio,HoraFin,Fecha,Estado,IdEmpleado")] Agenda agenda)
        {
            if (id != agenda.IdAgenda)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agenda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendaExists(agenda.IdAgenda))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", agenda.IdEmpleado);
            return View(agenda);
        }

        // GET: Agenda/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Agendas == null)
            {
                return NotFound();
            }

            var agenda = await _context.Agendas
                .Include(a => a.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdAgenda == id);
            if (agenda == null)
            {
                return NotFound();
            }

            return View(agenda);
        }

        // POST: Agenda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Agendas == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.Agendas'  is null.");
            }
            var agenda = await _context.Agendas.FindAsync(id);
            if (agenda != null)
            {
                _context.Agendas.Remove(agenda);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgendaExists(int id)
        {
          return (_context.Agendas?.Any(e => e.IdAgenda == id)).GetValueOrDefault();
        }
    }
}
