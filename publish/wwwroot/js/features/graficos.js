/**
 * Comparación entre presupuestos y ejecución de gastos mediante gráficos
 */
document.addEventListener('DOMContentLoaded', function () {
  if (typeof chartData !== 'undefined' && chartData.length > 0) {
    var labels = chartData.map(item => item.TipoGastoNombre);
    var presupuestoData = chartData.map(item => item.MontoPresupuestado);
    var ejecucionData = chartData.map(item => item.MontoEjecutado);

    var ctx = document.getElementById('budgetExecutionChart').getContext('2d');
    var budgetExecutionChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [
          {
            label: 'Presupuestado',
            data: presupuestoData,
            backgroundColor: 'rgba(168, 213, 226, 0.6)',
            borderColor: 'rgba(168, 213, 226, 1)',
            borderWidth: 1
          },
          {
            label: 'Ejecutado',
            data: ejecucionData,
            backgroundColor: 'rgba(248, 179, 179, 0.6)',
            borderColor: 'rgba(248, 179, 179, 1)',
            borderWidth: 1
          }
        ]
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
            title: {
              display: true,
              text: 'Monto'
            }
          },
          x: {
            title: {
              display: true,
              text: 'Tipo de Gasto'
            }
          }
        },
        plugins: {
          title: {
            display: true,
            text: 'Comparativa de Presupuesto vs. Ejecución por Tipo de Gasto'
          },
          tooltip: {
            callbacks: {
              label: function (context) {
                let label = context.dataset.label || '';
                if (label) {
                  label += ': ';
                }
                if (context.parsed.y !== null) {
                  label += new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP' }).format(context.parsed.y);
                }
                return label;
              }
            }
          }
        }
      }
    });
  }
});
