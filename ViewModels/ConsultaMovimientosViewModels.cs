using System.ComponentModel.DataAnnotations;

namespace Project_AG.ViewModels
{    /// <summary>
    /// Modelo de vista para el formulario de consulta de movimientos por rango de fechas
    /// </summary>
    public class ConsultaMovimientosInputViewModel
    {
        [Required(ErrorMessage = "La fecha inicial es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicial")]
        public DateTime FechaInicio { get; set; } = DateTime.Today.AddMonths(-1);

        [Required(ErrorMessage = "La fecha final es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Final")]
        public DateTime FechaFin { get; set; } = DateTime.Today;

        public List<MovimientoViewModel> Movimientos { get; set; } = new();

        [Display(Name = "Total Ingresos")]
        [DataType(DataType.Currency)]
        public decimal TotalIngresos => Movimientos.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);

        [Display(Name = "Total Egresos")]
        [DataType(DataType.Currency)]
        public decimal TotalEgresos => Movimientos.Where(m => m.Tipo == "Egreso").Sum(m => m.Monto);

        [Display(Name = "Balance")]
        [DataType(DataType.Currency)]
        public decimal Balance => TotalIngresos - TotalEgresos;
    }    /// <summary>
    /// Modelo de vista que representa un movimiento financiero, ya sea un gasto o un depósito
    /// </summary>
    public class MovimientoViewModel
    {
        public DateTime Fecha { get; set; }

        [Display(Name = "Fondo Monetario")]
        public string FondoMonetarioNombre { get; set; }

        /// <summary>
        /// Descripción del movimiento en formato "Gasto: TipoGasto - Comercio" o "Depósito"
        /// </summary>
        public string Descripcion { get; set; }

        [DataType(DataType.Currency)]
        public decimal Monto { get; set; }

        /// <summary>
        /// Tipo de movimiento: "Ingreso" o "Egreso"
        /// </summary>
        public string Tipo { get; set; }
    }
}