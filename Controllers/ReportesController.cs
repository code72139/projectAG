using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_AG.Data;
using Project_AG.ViewModels;
using Project_AG.Filters;

namespace Project_AG.Controllers
{
    /// <summary>
    /// Gestiona la generación y visualización de informes y gráficos
    /// </summary>
    [Autenticado]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }
        public List<ChartDataPoint> ChartData { get; set; } = new List<ChartDataPoint>();

        public IActionResult GraficoComparativo()
        {
            var viewModel = new GraficoComparativoViewModel
            {
                MesInicio = DateTime.Today.Month,
                AnioInicio = DateTime.Today.Year,
                MesFin = DateTime.Today.Month,
                AnioFin = DateTime.Today.Year
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GraficoComparativo(GraficoComparativoViewModel viewModel)
        {
            ModelState.Clear();

            DateTime fechaInicio = new DateTime(viewModel.AnioInicio, viewModel.MesInicio, 1);
            DateTime fechaFin = new DateTime(viewModel.AnioFin, viewModel.MesFin, DateTime.DaysInMonth(viewModel.AnioFin, viewModel.MesFin));

            if (fechaInicio > fechaFin)
            {
                viewModel.MensajeError = "La fecha de inicio no puede ser posterior a la fecha fin.";
                return View(viewModel);
            }

            fechaFin = fechaFin.AddDays(1).AddTicks(-1);

            viewModel.MensajeError = $"Rango seleccionado: {fechaInicio.ToShortDateString()} a {fechaFin.ToShortDateString()}";

            var tiposGasto = await _context.TiposGasto.ToListAsync();

            var presupuestosPorTipo = await _context.PresupuestosTipoGasto
                .Where(p =>
                    (p.Anio > fechaInicio.Year || (p.Anio == fechaInicio.Year && p.Mes >= fechaInicio.Month)) &&
                    (p.Anio < fechaFin.Year || (p.Anio == fechaFin.Year && p.Mes <= fechaFin.Month))
                )
                .GroupBy(p => p.TipoGastoId)
                .Select(g => new
                {
                    TipoGastoId = g.Key,
                    MontoPresupuestado = g.Sum(p => p.MontoPresupuestado)
                })
                .ToListAsync();

            var ejecucionPorTipo = await _context.DetallesGasto
                .Include(dg => dg.RegistroGasto)
                .Where(dg => dg.RegistroGasto.Fecha >= fechaInicio && dg.RegistroGasto.Fecha <= fechaFin)
                .GroupBy(dg => dg.TipoGastoId)
                .Select(g => new
                {
                    TipoGastoId = g.Key,
                    MontoEjecutado = g.Sum(dg => dg.Monto)
                })
                .ToListAsync();

            var chartData = new List<ChartDataPoint>();
            foreach (var tipoGasto in tiposGasto)
            {
                var presupuesto = presupuestosPorTipo.FirstOrDefault(p => p.TipoGastoId == tipoGasto.Id)?.MontoPresupuestado ?? 0;
                var ejecucion = ejecucionPorTipo.FirstOrDefault(e => e.TipoGastoId == tipoGasto.Id)?.MontoEjecutado ?? 0;

                if (presupuesto > 0 || ejecucion > 0)
                {
                    chartData.Add(new ChartDataPoint
                    {
                        TipoGastoNombre = tipoGasto.Nombre,
                        MontoPresupuestado = presupuesto,
                        MontoEjecutado = ejecucion
                    });
                }
            }

            viewModel.ChartData = chartData;

            if (!viewModel.ChartData.Any())
            {
                viewModel.MensajeError = "No se encontraron datos de presupuesto o ejecución para el rango de fechas seleccionado.";
            }

            viewModel.MesInicio = viewModel.MesInicio == 0 ? DateTime.Today.Month : viewModel.MesInicio;
            viewModel.AnioInicio = viewModel.AnioInicio == 0 ? DateTime.Today.Year : viewModel.AnioInicio;
            viewModel.MesFin = viewModel.MesFin == 0 ? DateTime.Today.Month : viewModel.MesFin;
            viewModel.AnioFin = viewModel.AnioFin == 0 ? DateTime.Today.Year : viewModel.AnioFin;

            return View(viewModel);
        }
    }
}