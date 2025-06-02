using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_AG.Data;
using Project_AG.Models;
using Project_AG.ViewModels;
using Project_AG.Filters;

namespace Project_AG.Controllers
{
    /// <summary>
    /// Gestiona las operaciones relacionadas con el registro de gastos y sus detalles
    /// </summary>
    [Autenticado]
    public class RegistroGastosController : Controller
    {
        private readonly AppDbContext _context;

        public RegistroGastosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var registros = await _context.RegistrosGasto
                .Include(r => r.FondoMonetario)
                .ToListAsync();
            return View(registros);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var registroGasto = await _context.RegistrosGasto
                .Include(r => r.FondoMonetario)
                .Include(r => r.DetallesGasto)
                    .ThenInclude(d => d.TipoGasto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (registroGasto == null) return NotFound();

            var viewModel = MapToDetailsViewModel(registroGasto);

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new RegistroGastoCreateViewModel
            {
                Fecha = DateTime.Today,
                FondosDisponibles = GetFondosSelectList(),
                TiposDeGastoDisponibles = GetTiposGastoSelectList()
            };
            viewModel.DetallesGasto.Add(new DetalleGastoCreateEditViewModel());
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroGastoCreateViewModel viewModel)
        {
            if (!ModelState.IsValid || !viewModel.DetallesGasto.Any(d => d.Monto > 0))
            {
                if (!viewModel.DetallesGasto.Any(d => d.Monto > 0))
                {
                    ModelState.AddModelError(string.Empty, "Debe ingresar al menos un detalle de gasto con un monto mayor a cero.");
                }
                RecargarListas(viewModel);
                return View(viewModel);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var registro = MapToRegistroGasto(viewModel);

                    _context.RegistrosGasto.Add(registro);
                    await _context.SaveChangesAsync();

                    var erroresPresupuesto = await ValidarSobregiroPresupuestoAsync(registro);
                    if (erroresPresupuesto.Any())
                    {
                        foreach (var error in erroresPresupuesto)
                            ModelState.AddModelError(string.Empty, error);

                        await transaction.RollbackAsync();
                        RecargarListas(viewModel);
                        return View(viewModel);
                    }

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Gasto registrado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al guardar el gasto. Por favor, intente de nuevo.");
                    RecargarListas(viewModel);
                    return View(viewModel);
                }
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var registroGasto = await _context.RegistrosGasto
                .Include(r => r.DetallesGasto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (registroGasto == null) return NotFound();

            var viewModel = new RegistroGastoCreateViewModel
            {
                Id = registroGasto.Id,
                Fecha = registroGasto.Fecha,
                FondoMonetarioId = registroGasto.FondoMonetarioId,
                Observaciones = registroGasto.Observaciones,
                NombreComercio = registroGasto.NombreComercio,
                TipoDocumento = registroGasto.TipoDocumento,
                DetallesGasto = registroGasto.DetallesGasto.Select(d => new DetalleGastoCreateEditViewModel
                {
                    Id = d.Id,
                    TipoGastoId = d.TipoGastoId,
                    Monto = d.Monto
                }).ToList()
            };

            RecargarListas(viewModel);
            if (!viewModel.DetallesGasto.Any())
            {
                viewModel.DetallesGasto.Add(new DetalleGastoCreateEditViewModel());
            }
            return View(viewModel);
        }

        // POST: RegistroGastos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegistroGastoCreateViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (!ModelState.IsValid || !viewModel.DetallesGasto.Any(d => d.Monto > 0))
            {
                if (!viewModel.DetallesGasto.Any(d => d.Monto > 0))
                {
                    ModelState.AddModelError(string.Empty, "Debe ingresar al menos un detalle de gasto con un monto mayor a cero.");
                }
                RecargarListas(viewModel);
                return View(viewModel);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var registroExistente = await _context.RegistrosGasto
                        .Include(r => r.DetallesGasto)
                        .FirstOrDefaultAsync(r => r.Id == id);

                    if (registroExistente == null)
                    {
                        await transaction.RollbackAsync();
                        return NotFound();
                    }

                    registroExistente.Fecha = viewModel.Fecha;
                    registroExistente.FondoMonetarioId = viewModel.FondoMonetarioId;
                    registroExistente.Observaciones = viewModel.Observaciones;
                    registroExistente.NombreComercio = viewModel.NombreComercio;
                    registroExistente.TipoDocumento = viewModel.TipoDocumento;

                    var detallesExistentesIds = registroExistente.DetallesGasto.Select(d => d.Id).ToList();
                    var detallesViewModelIds = viewModel.DetallesGasto.Where(d => d.Id != 0).Select(d => d.Id).ToList();

                    var detallesAEliminar = registroExistente.DetallesGasto
                        .Where(d => !detallesViewModelIds.Contains(d.Id))
                        .ToList();

                    _context.DetallesGasto.RemoveRange(detallesAEliminar);

                    foreach (var detalleVm in viewModel.DetallesGasto)
                    {
                        if (detalleVm.Id == 0)
                        {
                            registroExistente.DetallesGasto.Add(new DetalleGasto
                            {
                                TipoGastoId = detalleVm.TipoGastoId,
                                Monto = detalleVm.Monto
                            });
                        }
                        else
                        {
                            var detalleExistente = registroExistente.DetallesGasto
                                .FirstOrDefault(d => d.Id == detalleVm.Id);

                            if (detalleExistente != null)
                            {
                                detalleExistente.TipoGastoId = detalleVm.TipoGastoId;
                                detalleExistente.Monto = detalleVm.Monto;
                                _context.Entry(detalleExistente).State = EntityState.Modified;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    var erroresPresupuesto = await ValidarSobregiroPresupuestoAsync(registroExistente);
                    if (erroresPresupuesto.Any())
                    {
                        foreach (var error in erroresPresupuesto)
                            ModelState.AddModelError(string.Empty, error);

                        await transaction.RollbackAsync();
                        RecargarListas(viewModel);
                        return View(viewModel);
                    }

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Gasto actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    if (!RegistroGastoExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Hubo un conflicto de concurrencia al actualizar el gasto. Por favor, intente de nuevo.");
                        RecargarListas(viewModel);
                        return View(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar el gasto. Por favor, intente de nuevo.");
                    RecargarListas(viewModel);
                    return View(viewModel);
                }
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var registroGasto = await _context.RegistrosGasto
                .Include(r => r.FondoMonetario)
                .Include(r => r.DetallesGasto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (registroGasto == null) return NotFound();
            return View(registroGasto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registroGasto = await _context.RegistrosGasto
                .Include(r => r.DetallesGasto)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registroGasto != null)
            {
                _context.RegistrosGasto.Remove(registroGasto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Gasto eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ConsultaMovimientos()
        {
            var viewModel = new ConsultaMovimientosInputViewModel
            {
                FechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                FechaFin = DateTime.Today
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConsultaMovimientos(ConsultaMovimientosInputViewModel inputViewModel)
        {
            if (ModelState.IsValid)
            {    

                DateTime fechaInicio = inputViewModel.FechaInicio.Date;
                DateTime fechaFin = inputViewModel.FechaFin.Date;

                if (fechaInicio > fechaFin)
                {
                    ModelState.AddModelError(string.Empty, "La fecha de inicio no puede ser posterior a la fecha fin.");
                    return View(inputViewModel);
                }

                var gastos = await _context.RegistrosGasto
                    .Include(rg => rg.FondoMonetario)
                    .Include(rg => rg.DetallesGasto)
                        .ThenInclude(dg => dg.TipoGasto)
                    .Where(rg => rg.Fecha >= fechaInicio && rg.Fecha <= fechaFin)
                    .SelectMany(rg => rg.DetallesGasto.Select(dg => new MovimientoViewModel
                    {
                        Fecha = rg.Fecha,
                        FondoMonetarioNombre = rg.FondoMonetario.NombreFondo,
                        Descripcion = $"Gasto: {dg.TipoGasto.Nombre} ({rg.NombreComercio})",
                        Monto = dg.Monto,
                        Tipo = "Egreso"
                    }))
                    .ToListAsync();

                var depositos = await _context.Depositos
                    .Include(d => d.FondoMonetario)
                    .Where(d => d.FechaDeposito >= fechaInicio && d.FechaDeposito <= fechaFin)
                    .Select(d => new MovimientoViewModel
                    {
                        Fecha = d.FechaDeposito,
                        FondoMonetarioNombre = d.FondoMonetario.NombreFondo,
                        Descripcion = "Depósito",
                        Monto = d.Monto,
                        Tipo = "Ingreso"
                    })
                    .ToListAsync();

                inputViewModel.Movimientos = gastos
                    .Concat(depositos)
                    .OrderBy(m => m.Fecha)
                    .ThenBy(m => m.Tipo)
                    .ToList();
            }

            return View(inputViewModel);
        }


        private bool RegistroGastoExists(int id)
            => _context.RegistrosGasto.Any(e => e.Id == id);

        private RegistroGastoDetailsViewModel MapToDetailsViewModel(RegistroGasto registro)
            => new RegistroGastoDetailsViewModel
            {
                Id = registro.Id,
                Fecha = registro.Fecha,
                NombreComercio = registro.NombreComercio,
                Observaciones = registro.Observaciones,
                TipoDocumento = registro.TipoDocumento.ToString(),
                NombreFondo = registro.FondoMonetario.NombreFondo,
                DetallesGasto = registro.DetallesGasto.Select(d => new DetalleGastoDetailsViewModel
                {
                    TipoGastoNombre = d.TipoGasto.Nombre,
                    Monto = d.Monto
                }).ToList()
            };

        private List<SelectListItem> GetFondosSelectList()
            => _context.FondosMonetarios
                .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.NombreFondo })
                .ToList();

        private List<SelectListItem> GetTiposGastoSelectList()
            => _context.TiposGasto
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre })
                .ToList();

        private void RecargarListas(RegistroGastoCreateViewModel viewModel)
        {
            viewModel.FondosDisponibles = GetFondosSelectList();
            viewModel.TiposDeGastoDisponibles = GetTiposGastoSelectList();
        }

        private RegistroGasto MapToRegistroGasto(RegistroGastoCreateViewModel viewModel)
            => new RegistroGasto
            {
                Fecha = viewModel.Fecha,
                FondoMonetarioId = viewModel.FondoMonetarioId,
                Observaciones = viewModel.Observaciones,
                NombreComercio = viewModel.NombreComercio,
                TipoDocumento = viewModel.TipoDocumento, // No need for explicit cast if enum types match
                DetallesGasto = viewModel.DetallesGasto.Select(d => new DetalleGasto
                {
                    TipoGastoId = d.TipoGastoId,
                    Monto = d.Monto
                }).ToList()
            };

        private async Task<List<string>> ValidarSobregiroPresupuestoAsync(RegistroGasto registro)
        {
            var errores = new List<string>();

            var tiposGastoIds = registro.DetallesGasto.Select(d => d.TipoGastoId).Distinct().ToList();
            var mes = registro.Fecha.Month;
            var anio = registro.Fecha.Year;

            var presupuestos = await _context.PresupuestosTipoGasto
                .Where(p => tiposGastoIds.Contains(p.TipoGastoId) && p.Mes == mes && p.Anio == anio)
                .GroupBy(p => p.TipoGastoId)
                .Select(g => new
                {
                    TipoGastoId = g.Key,
                    MontoPresupuestado = g.Sum(x => x.MontoPresupuestado)
                }).ToListAsync();

            var gastosRegistradosQuery = _context.DetallesGasto
                .Include(d => d.RegistroGasto)
                .Where(d => tiposGastoIds.Contains(d.TipoGastoId)
                             && d.RegistroGasto.Fecha.Month == mes
                             && d.RegistroGasto.Fecha.Year == anio);

            if (registro.Id != 0)
                gastosRegistradosQuery = gastosRegistradosQuery.Where(d => d.RegistroGastoId != registro.Id);

            var gastosRegistrados = await gastosRegistradosQuery
                .GroupBy(d => d.TipoGastoId)
                .Select(g => new
                {
                    TipoGastoId = g.Key,
                    MontoGastado = g.Sum(x => x.Monto)
                }).ToListAsync();

            foreach (var detalle in registro.DetallesGasto)
            {
                var presupuesto = presupuestos.FirstOrDefault(p => p.TipoGastoId == detalle.TipoGastoId)?.MontoPresupuestado ?? 0;
                var gastado = gastosRegistrados.FirstOrDefault(g => g.TipoGastoId == detalle.TipoGastoId)?.MontoGastado ?? 0;
                var totalConNuevoGasto = gastado + detalle.Monto;

                if (totalConNuevoGasto > presupuesto)
                {
                    var tipoGastoNombre = (await _context.TiposGasto.FirstOrDefaultAsync(t => t.Id == detalle.TipoGastoId))?.Nombre ?? "Tipo de Gasto Desconocido";
                    errores.Add($"Presupuesto sobregirado para '{tipoGastoNombre}': presupuestado {presupuesto:C}, gastado hasta ahora {gastado:C}, nuevo gasto {detalle.Monto:C}.");
                }
            }
            return errores;
        }
    }
}