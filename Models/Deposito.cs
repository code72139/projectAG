using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_AG.Models
{
    /// <summary>
    /// Representa un depósito monetario realizado a un fondo específico
    /// </summary>
    public class Deposito
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha del depósito es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Depósito")]
        public DateTime FechaDeposito { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Required]
        [Display(Name = "Fondo Monetario")]
        public int FondoMonetarioId { get; set; }

        [ForeignKey("FondoMonetarioId")]
        public FondoMonetario? FondoMonetario { get; set; }
    }
}
