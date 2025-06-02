using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_AG.Data;
using Project_AG.Filters;
using Project_AG.Models;

namespace Project_AG.Controllers
{
    [Autenticado]
    public class TipoGastoController : Controller
    {
        private readonly AppDbContext _context;

        public TipoGastoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposGasto.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoGasto = await _context.TiposGasto
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tipoGasto == null)
            {
                return NotFound();
            }

            return View(tipoGasto);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.CodigoAutomatico = await GenerarSiguienteCodigoTipoGasto();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,Nombre")] TipoGasto tipoGasto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CodigoAutomatico = await GenerarSiguienteCodigoTipoGasto();
                return View(tipoGasto);
            }

            bool existeNombreDuplicado = await _context.TiposGasto.AnyAsync(t =>
                t.Nombre == tipoGasto.Nombre);

            if (existeNombreDuplicado)
            {
                ModelState.AddModelError("Nombre", "Ya existe un tipo de gasto con este nombre. Por favor, elija un nombre diferente.");
                ViewBag.CodigoAutomatico = await GenerarSiguienteCodigoTipoGasto();
                return View(tipoGasto);
            }

            tipoGasto.Codigo = await GenerarSiguienteCodigoTipoGasto();

            _context.Add(tipoGasto);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al guardar el tipo de gasto. Intente de nuevo.");
                ViewBag.CodigoAutomatico = await GenerarSiguienteCodigoTipoGasto();
                return View(tipoGasto);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoGasto = await _context.TiposGasto.FindAsync(id);
            if (tipoGasto == null)
            {
                return NotFound();
            }
            return View(tipoGasto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre")] TipoGasto tipoGasto)
        {
            if (id != tipoGasto.Id)
            {
                return NotFound();
            }

            var tipoGastoEnDb = await _context.TiposGasto.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (tipoGastoEnDb == null)
            {
                return NotFound();
            }
            tipoGasto.Codigo = tipoGastoEnDb.Codigo;

            if (!ModelState.IsValid)
            {
                return View(tipoGasto);
            }

            bool existeNombreDuplicado = await _context.TiposGasto.AnyAsync(t =>
                t.Nombre == tipoGasto.Nombre && t.Id != tipoGasto.Id);

            if (existeNombreDuplicado)
            {
                ModelState.AddModelError("Nombre", "Ya existe otro tipo de gasto con este nombre. Por favor, elija un nombre diferente.");
                return View(tipoGasto);
            }

            try
            {
                _context.Update(tipoGasto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoGastoExists(tipoGasto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al guardar los cambios. Intente de nuevo.");
                return View(tipoGasto);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoGasto = await _context.TiposGasto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoGasto == null)
            {
                return NotFound();
            }
            return View(tipoGasto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoGasto = await _context.TiposGasto.FindAsync(id);
            if (tipoGasto != null)
            {
                _context.TiposGasto.Remove(tipoGasto);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "No se pudo eliminar el tipo de gasto. Podría estar siendo utilizado en otros registros (ej. en gastos o presupuestos).");
                    var tipoGastoParaError = await _context.TiposGasto.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    return View("Delete", tipoGastoParaError);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoGastoExists(int id)
        {
            return _context.TiposGasto.Any(e => e.Id == id);
        }

        private async Task<string> GenerarSiguienteCodigoTipoGasto()
        {
            var ultimoCodigo = await _context.TiposGasto
                .OrderByDescending(t => t.Codigo)
                .Select(t => t.Codigo)
                .FirstOrDefaultAsync();

            int siguienteCodigo = 1;
            if (!string.IsNullOrEmpty(ultimoCodigo) && ultimoCodigo.StartsWith("TP-") && int.TryParse(ultimoCodigo.Substring(3), out int codigoNumerico))
            {
                siguienteCodigo = codigoNumerico + 1;
            }
            return $"TP-{siguienteCodigo:D3}";
        }
    }
}