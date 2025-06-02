using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_AG.Models
{
    /// <summary>
    /// Representa un detalle individual de un gasto con su tipo y monto
    /// </summary>
    public class DetalleGasto
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Registro Gasto")]
        public int RegistroGastoId { get; set; }

        [ForeignKey("RegistroGastoId")]
        public RegistroGasto? RegistroGasto { get; set; }

        [Required(ErrorMessage = "El tipo de gasto es obligatorio")]
        [Display(Name = "Tipo de Gasto")]
        public int TipoGastoId { get; set; }

        [ForeignKey("TipoGastoId")]
        public TipoGasto? TipoGasto { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, 9999999.99, ErrorMessage = "El monto debe ser entre 0.01 y 9,999,999.99")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }
    }
}
