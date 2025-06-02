using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_AG.ViewModels
{
    /// <summary>
    /// Modelo de vista para la generación de gráficos comparativos de presupuesto vs ejecución
    /// </summary>
    public class GraficoComparativoViewModel
    {
        [Required(ErrorMessage = "El mes de inicio es requerido.")]
        [Range(1, 12, ErrorMessage = "Mes de inicio inválido.")]
        [Display(Name = "Mes Inicio")]
        public int MesInicio { get; set; }

        [Required(ErrorMessage = "El año de inicio es requerido.")]
        [Range(1900, 2100, ErrorMessage = "Año de inicio inválido.")]
        [Display(Name = "Año Inicio")]
        public int AnioInicio { get; set; }

        [Required(ErrorMessage = "El mes fin es requerido.")]
        [Range(1, 12, ErrorMessage = "Mes fin inválido.")]
        [Display(Name = "Mes Fin")]
        public int MesFin { get; set; }

        [Required(ErrorMessage = "El año fin es requerido.")]
        [Range(1900, 2100, ErrorMessage = "Año fin inválido.")]
        [Display(Name = "Año Fin")]
        public int AnioFin { get; set; }        /// <summary>
        /// Datos para generar el gráfico comparativo
        /// </summary>
        public List<ChartDataPoint> ChartData { get; set; } = new List<ChartDataPoint>();

        /// <summary>
        /// Mensaje de error si la consulta falla
        /// </summary>
        public string MensajeError { get; set; }
    }

    /// <summary>
    /// Representa un punto de datos en el gráfico comparativo
    /// </summary>
    public class ChartDataPoint
    {
        public string TipoGastoNombre { get; set; }
        public decimal MontoPresupuestado { get; set; }
        public decimal MontoEjecutado { get; set; }
    }
}