using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_AG.Data;
using Project_AG.Models;
using Project_AG.Filters;

namespace Project_AG.Controllers
{
    /// <summary>
    /// Gestiona las operaciones de depósitos monetarios
    /// </summary>
    [Autenticado]
    public class DepositosController : Controller
    {
        private readonly AppDbContext _context;
        public DepositosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Depositos.Include(d => d.FondoMonetario);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposito = await _context.Depositos
                .Include(d => d.FondoMonetario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }

        public IActionResult Create()
        {
            ViewData["FondoMonetarioId"] = new SelectList(_context.FondosMonetarios, "Id", "NombreFondo");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaDeposito,Monto,FondoMonetarioId")] Deposito deposito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deposito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FondoMonetarioId"] = new SelectList(_context.FondosMonetarios, "Id", "NombreFondo", deposito.FondoMonetarioId);
            return View(deposito);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito == null)
            {
                return NotFound();
            }
            ViewData["FondoMonetarioId"] = new SelectList(_context.FondosMonetarios, "Id", "NombreFondo", deposito.FondoMonetarioId);
            return View(deposito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaDeposito,Monto,FondoMonetarioId")] Deposito deposito)
        {
            if (id != deposito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deposito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositoExists(deposito.Id))
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
            ViewData["FondoMonetarioId"] = new SelectList(_context.FondosMonetarios, "Id", "NombreFondo", deposito.FondoMonetarioId);
            return View(deposito);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposito = await _context.Depositos
                .Include(d => d.FondoMonetario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deposito == null)
            {
                return NotFound();
            }

            return View(deposito);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito != null)
            {
                _context.Depositos.Remove(deposito);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepositoExists(int id)
        {
            return _context.Depositos.Any(e => e.Id == id);
        }
    }
}
