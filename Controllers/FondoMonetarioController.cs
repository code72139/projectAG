using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_AG.Data;
using Project_AG.Filters;
using Project_AG.Models;

namespace Project_AG.Controllers
{
    /// <summary>
    /// Gestiona las operaciones relacionadas con los fondos monetarios
    /// </summary>
    [Autenticado]
    public class FondoMonetarioController : Controller
    {
        private readonly AppDbContext _context;

        public FondoMonetarioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.FondosMonetarios.ToListAsync());        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fondoMonetario = await _context.FondosMonetarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fondoMonetario == null)
            {
                return NotFound();
            }

            return View(fondoMonetario);        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreFondo,TipoFondoMonetario,DescripcionFondo,EstaActivo,FechaCreacion")] FondoMonetario fondoMonetario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fondoMonetario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fondoMonetario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fondoMonetario = await _context.FondosMonetarios.FindAsync(id);
            if (fondoMonetario == null)
            {
                return NotFound();
            }
            return View(fondoMonetario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreFondo,TipoFondoMonetario,DescripcionFondo,EstaActivo,FechaCreacion")] FondoMonetario fondoMonetario)
        {
            if (id != fondoMonetario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fondoMonetario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FondoMonetarioExists(fondoMonetario.Id))
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
            return View(fondoMonetario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fondoMonetario = await _context.FondosMonetarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fondoMonetario == null)
            {
                return NotFound();
            }

            return View(fondoMonetario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fondoMonetario = await _context.FondosMonetarios.FindAsync(id);
            if (fondoMonetario != null)
            {
                _context.FondosMonetarios.Remove(fondoMonetario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FondoMonetarioExists(int id)
        {
            return _context.FondosMonetarios.Any(e => e.Id == id);
        }
    }
}
