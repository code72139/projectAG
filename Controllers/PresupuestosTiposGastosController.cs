using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project_AG.Data;
using Project_AG.Models;
using Project_AG.Filters;

namespace Project_AG.Controllers
{
    /// <summary>
    /// Gestiona la asignación y control de presupuestos por tipos de gastos
    /// </summary>
    [Autenticado]
    public class PresupuestosTiposGastosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PresupuestosTiposGastosController> _logger;

        public PresupuestosTiposGastosController(AppDbContext context, ILogger<PresupuestosTiposGastosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.PresupuestosTipoGasto.Include(p => p.TipoGasto).Include(p => p.Usuario);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Detalles de PresupuestoTipoGasto solicitado sin ID.");
                return NotFound();
            }

            var presupuestoTipoGasto = await _context.PresupuestosTipoGasto
                .Include(p => p.TipoGasto)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (presupuestoTipoGasto == null)
            {
                _logger.LogWarning($"PresupuestoTipoGasto con ID {id} no encontrado para detalles.");
                return NotFound();
            }

            return View(presupuestoTipoGasto);
        }

        public IActionResult Create()
        {
            ViewBag.TipoGastoId = new SelectList(_context.TiposGasto, "Id", "Nombre");
            ViewBag.UsuarioId = new SelectList(_context.Usuarios, "Id", "NombreUsuario");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PresupuestoTipoGasto presupuestoTipoGasto)
        {
            _logger.LogInformation("Entrando al método POST Create de PresupuestosTiposGastos.");

            _logger.LogInformation($"Datos recibidos: UsuarioId={presupuestoTipoGasto.UsuarioId}, TipoGastoId={presupuestoTipoGasto.TipoGastoId}, Mes={presupuestoTipoGasto.Mes}, Año={presupuestoTipoGasto.Anio}, Monto={presupuestoTipoGasto.MontoPresupuestado}");

            bool existePresupuestoDuplicado = await _context.PresupuestosTipoGasto.AnyAsync(p =>
                p.UsuarioId == presupuestoTipoGasto.UsuarioId &&
                p.TipoGastoId == presupuestoTipoGasto.TipoGastoId &&
                p.Mes == presupuestoTipoGasto.Mes &&
                p.Anio == presupuestoTipoGasto.Anio);

            if (existePresupuestoDuplicado)
            {
                ModelState.AddModelError(string.Empty, "Ya existe un presupuesto para este usuario, tipo de gasto, mes y año. Por favor, edite el presupuesto existente o elija una combinación diferente.");
                _logger.LogWarning($"Intento de crear presupuesto duplicado para UsuarioId={presupuestoTipoGasto.UsuarioId}, TipoGastoId={presupuestoTipoGasto.TipoGastoId}, Mes={presupuestoTipoGasto.Mes}, Año={presupuestoTipoGasto.Anio}");
            }

            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Errores en el modelo (incluyendo unicidad): {@errores}", errores);

                ViewBag.TipoGastoId = new SelectList(_context.TiposGasto, "Id", "Nombre", presupuestoTipoGasto.TipoGastoId);
                ViewBag.UsuarioId = new SelectList(_context.Usuarios, "Id", "NombreUsuario", presupuestoTipoGasto.UsuarioId);
                return View(presupuestoTipoGasto);
            }

            _context.Add(presupuestoTipoGasto);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Presupuesto guardado correctamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar el presupuesto: {ErrorMessage}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Ocurrió un error al guardar el presupuesto. Verifique los datos o intente de nuevo.");

                ViewBag.TipoGastoId = new SelectList(_context.TiposGasto, "Id", "Nombre", presupuestoTipoGasto.TipoGastoId);
                ViewBag.UsuarioId = new SelectList(_context.Usuarios, "Id", "NombreUsuario", presupuestoTipoGasto.UsuarioId);
                return View(presupuestoTipoGasto);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edición de PresupuestoTipoGasto solicitada sin ID.");
                return NotFound();
            }

            var presupuestoTipoGasto = await _context.PresupuestosTipoGasto.FindAsync(id);
            if (presupuestoTipoGasto == null)
            {
                _logger.LogWarning($"PresupuestoTipoGasto con ID {id} no encontrado para edición.");
                return NotFound();
            }
            ViewData["TipoGastoId"] = new SelectList(_context.TiposGasto, "Id", "Nombre", presupuestoTipoGasto.TipoGastoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", presupuestoTipoGasto.UsuarioId);
            return View(presupuestoTipoGasto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsuarioId,TipoGastoId,Mes,Anio,MontoPresupuestado")] PresupuestoTipoGasto presupuestoTipoGasto)
        {
            _logger.LogInformation($"Entrando al método POST Edit para ID: {id}");

            if (id != presupuestoTipoGasto.Id)
            {
                _logger.LogWarning($"Conflicto de ID: ID de URL ({id}) no coincide con ID de modelo ({presupuestoTipoGasto.Id})");
                return NotFound();
            }

            bool existePresupuestoDuplicado = await _context.PresupuestosTipoGasto.AnyAsync(p =>
                p.UsuarioId == presupuestoTipoGasto.UsuarioId &&
                p.TipoGastoId == presupuestoTipoGasto.TipoGastoId &&
                p.Mes == presupuestoTipoGasto.Mes &&
                p.Anio == presupuestoTipoGasto.Anio &&
                p.Id != presupuestoTipoGasto.Id);

            if (existePresupuestoDuplicado)
            {
                ModelState.AddModelError(string.Empty, "Ya existe otro presupuesto con la misma combinación de usuario, tipo de gasto, mes y año. Por favor, edite el presupuesto existente o elija una combinación diferente.");
                _logger.LogWarning($"Intento de editar presupuesto ID {id} a una combinación duplicada.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al intentar editar el presupuesto con ID {Id}. Errores: {Errores}", id, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
                ViewData["TipoGastoId"] = new SelectList(_context.TiposGasto, "Id", "Nombre", presupuestoTipoGasto.TipoGastoId);
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", presupuestoTipoGasto.UsuarioId);
                return View(presupuestoTipoGasto);
            }

            try
            {
                var presupuestoOriginal = await _context.PresupuestosTipoGasto.FindAsync(id);

                if (presupuestoOriginal == null)
                {
                    _logger.LogError($"No se encontró el presupuesto tipo gasto con ID {id} para editar (posible concurrencia).");
                    return NotFound();
                }

                presupuestoOriginal.TipoGastoId = presupuestoTipoGasto.TipoGastoId;
                presupuestoOriginal.Mes = presupuestoTipoGasto.Mes;
                presupuestoOriginal.Anio = presupuestoTipoGasto.Anio;
                presupuestoOriginal.MontoPresupuestado = presupuestoTipoGasto.MontoPresupuestado;


                await _context.SaveChangesAsync();
                _logger.LogInformation($"Presupuesto con ID {id} actualizado correctamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error de concurrencia al actualizar el presupuesto con ID {id}.");
                if (!PresupuestoTipoGastoExists(presupuestoTipoGasto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error de base de datos al intentar actualizar el presupuesto con ID {id}: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "Ocurrió un error al guardar los cambios. Verifique los datos o intente de nuevo.");
                ViewData["TipoGastoId"] = new SelectList(_context.TiposGasto, "Id", "Nombre", presupuestoTipoGasto.TipoGastoId);
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "NombreUsuario", presupuestoTipoGasto.UsuarioId);
                return View(presupuestoTipoGasto);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Eliminación de PresupuestoTipoGasto solicitada sin ID.");
                return NotFound();
            }

            var presupuestoTipoGasto = await _context.PresupuestosTipoGasto
                .Include(p => p.TipoGasto)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (presupuestoTipoGasto == null)
            {
                _logger.LogWarning($"PresupuestoTipoGasto con ID {id} no encontrado para eliminación.");
                return NotFound();
            }

            return View(presupuestoTipoGasto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Confirmando eliminación de PresupuestoTipoGasto con ID: {id}.");
            var presupuestoTipoGasto = await _context.PresupuestosTipoGasto.FindAsync(id);
            if (presupuestoTipoGasto != null)
            {
                _context.PresupuestosTipoGasto.Remove(presupuestoTipoGasto);
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"PresupuestoTipoGasto con ID {id} eliminado exitosamente.");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, $"Error de base de datos al eliminar PresupuestoTipoGasto con ID {id}: {ex.InnerException?.Message}");
                    ModelState.AddModelError("", "No se pudo eliminar el presupuesto. Podría estar relacionado con otros registros (ej. registros de gastos).");
                    var presupuestoParaError = await _context.PresupuestosTipoGasto.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    return View("Delete", presupuestoParaError);
                }
            }
            else
            {
                _logger.LogWarning($"Intento de eliminar PresupuestoTipoGasto con ID {id} fallido: no encontrado.");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PresupuestoTipoGastoExists(int id)
        {
            return _context.PresupuestosTipoGasto.Any(e => e.Id == id);
        }
    }
}