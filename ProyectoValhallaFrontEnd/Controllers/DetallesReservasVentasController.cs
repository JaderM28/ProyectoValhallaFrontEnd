using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoValhallaFrontEnd.Models;

namespace ValhallaProyecto.Controllers
{
    public class DetallesReservasVentasController : Controller
    {
        private readonly ValhallaDbContext _context;

        public DetallesReservasVentasController(ValhallaDbContext context)
        {
            _context = context;
        }

        // GET: DetallesReservasVentas
        public async Task<IActionResult> Index()
        {
            var valhallaProyectoContext = _context.DetallesReservasVentas.Include(d => d.IdReservaNavigation).Include(d => d.IdVentaNavigation);
            return View(await valhallaProyectoContext.ToListAsync());
        }

        // GET: DetallesReservasVentas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DetallesReservasVentas == null)
            {
                return NotFound();
            }

            var detallesReservasVenta = await _context.DetallesReservasVentas
                .Include(d => d.IdReservaNavigation)
                .Include(d => d.IdVentaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetallesReservasVentas == id);
            if (detallesReservasVenta == null)
            {
                return NotFound();
            }

            return View(detallesReservasVenta);
        }

        // GET: DetallesReservasVentas/Create
        public IActionResult Create()
        {
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva");
            ViewData["IdVenta"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta");
            return View();
        }

        // POST: DetallesReservasVentas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetallesReservasVentas,IdReserva,IdVenta")] DetallesReservasVenta detallesReservasVenta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallesReservasVenta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detallesReservasVenta.IdReserva);
            ViewData["IdVenta"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta", detallesReservasVenta.IdVenta);
            return View(detallesReservasVenta);
        }

        // GET: DetallesReservasVentas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DetallesReservasVentas == null)
            {
                return NotFound();
            }

            var detallesReservasVenta = await _context.DetallesReservasVentas.FindAsync(id);
            if (detallesReservasVenta == null)
            {
                return NotFound();
            }
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detallesReservasVenta.IdReserva);
            ViewData["IdVenta"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta", detallesReservasVenta.IdVenta);
            return View(detallesReservasVenta);
        }

        // POST: DetallesReservasVentas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetallesReservasVentas,IdReserva,IdVenta")] DetallesReservasVenta detallesReservasVenta)
        {
            if (id != detallesReservasVenta.IdDetallesReservasVentas)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallesReservasVenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallesReservasVentaExists(detallesReservasVenta.IdDetallesReservasVentas))
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
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detallesReservasVenta.IdReserva);
            ViewData["IdVenta"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta", detallesReservasVenta.IdVenta);
            return View(detallesReservasVenta);
        }

        // GET: DetallesReservasVentas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DetallesReservasVentas == null)
            {
                return NotFound();
            }

            var detallesReservasVenta = await _context.DetallesReservasVentas
                .Include(d => d.IdReservaNavigation)
                .Include(d => d.IdVentaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetallesReservasVentas == id);
            if (detallesReservasVenta == null)
            {
                return NotFound();
            }

            return View(detallesReservasVenta);
        }

        // POST: DetallesReservasVentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DetallesReservasVentas == null)
            {
                return Problem("Entity set 'ValhallaProyectoContext.DetallesReservasVentas'  is null.");
            }
            var detallesReservasVenta = await _context.DetallesReservasVentas.FindAsync(id);
            if (detallesReservasVenta != null)
            {
                _context.DetallesReservasVentas.Remove(detallesReservasVenta);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallesReservasVentaExists(int id)
        {
          return (_context.DetallesReservasVentas?.Any(e => e.IdDetallesReservasVentas == id)).GetValueOrDefault();
        }
    }
}
