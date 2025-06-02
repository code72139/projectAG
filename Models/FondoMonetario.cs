using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_AG.Models
{
    public class FondoMonetario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del fondo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre del fondo no debe superar los 100 caracteres")]
        [Display(Name = "Nombre del Fondo Monetario")]
        public string NombreFondo { get; set; }

        [Required(ErrorMessage = "El tipo de fondo es obligatorio")]
        [EnumDataType(typeof(TipoFondo))]
        [Display(Name = "Tipo de Fondo Monetario")]
        public TipoFondo TipoFondoMonetario { get; set; }

        [Display(Name = "Descripción del Fondo")]
        [StringLength(250, ErrorMessage = "La descripción del fondo no debe superar los 250 caracteres")]
        public string? DescripcionFondo { get; set; }

        [Required]
        [Display(Name = "¿Está Activo?")]
        public bool EstaActivo { get; set; } = true;

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;        /// <summary>
        /// Colección de depósitos asociados al fondo monetario
        /// </summary>
        public ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();

        /// <summary>
        /// Colección de registros de gastos asociados al fondo monetario
        /// </summary>
        public ICollection<RegistroGasto> RegistrosGasto { get; set; } = new List<RegistroGasto>();
    }

    public enum TipoFondo
    {
        [Display(Name = "Cuenta Bancaria")]
        CuentaBancaria,
        [Display(Name = "Caja Menuda")]
        CajaMenuda
    }
}
