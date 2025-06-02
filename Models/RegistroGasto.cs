using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_AG.Models
{
    public class RegistroGasto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha del gasto es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El fondo monetario es obligatorio")]
        [Display(Name = "Fondo Monetario")]
        public int FondoMonetarioId { get; set; }

        [ForeignKey("FondoMonetarioId")]
        public FondoMonetario? FondoMonetario { get; set; }

        [StringLength(200)]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El nombre del comercio es obligatorio")]
        [StringLength(100)]
        public string NombreComercio { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public TipoDocumento TipoDocumento { get; set; }        /// <summary>
        /// Colección de detalles asociados al registro de gasto
        /// </summary>
        public ICollection<DetalleGasto> DetallesGasto { get; set; } = new List<DetalleGasto>();
    }

    public enum TipoDocumento
    {
        Comprobante,
        Factura,
        Otro
    }
}
