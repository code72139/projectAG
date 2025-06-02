using Project_AG.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_AG.Models
{    /// <summary>
    /// Representa un presupuesto asignado a un tipo de gasto específico para un período determinado
    /// </summary>
    public class PresupuestoTipoGasto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario.")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de gasto.")]
        [Display(Name = "Tipo de Gasto")]
        public int TipoGastoId { get; set; }

        [ForeignKey("TipoGastoId")]
        /// <summary>
        /// Referencia al tipo de gasto asociado a este presupuesto
        /// </summary>
        public TipoGasto? TipoGasto { get; set; }

        [Required(ErrorMessage = "El campo Mes es obligatorio.")]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "El campo Año es obligatorio.")]
        [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100.")]
        [Display(Name = "Año")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "Debe ingresar un monto.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto Presupuestado")]
        public decimal MontoPresupuestado { get; set; }
    }
}