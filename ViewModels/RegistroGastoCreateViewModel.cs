using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project_AG.Models;

namespace Project_AG.ViewModels
{    /// <summary>
    /// Representa el modelo de vista para crear y editar registros de gastos
    /// </summary>
    public class RegistroGastoCreateViewModel
    {
        /// <summary>
        /// Identificador único del registro de gasto
        /// </summary>
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha del gasto es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del Gasto")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un fondo monetario.")]
        [Display(Name = "Fondo Monetario")]
        public int FondoMonetarioId { get; set; }

        [StringLength(200, ErrorMessage = "Las observaciones no pueden exceder los 200 caracteres.")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El nombre del comercio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del comercio no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre del Comercio")]
        public string NombreComercio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de documento.")]
        [Display(Name = "Tipo de Documento")]
        public TipoDocumento TipoDocumento { get; set; }

        [Display(Name = "Detalles del Gasto")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un detalle de gasto.")]
        public List<DetalleGastoCreateEditViewModel> DetallesGasto { get; set; } = new();

        // Para listas desplegables
        public List<SelectListItem>? FondosDisponibles { get; set; }
        public List<SelectListItem>? TiposDeGastoDisponibles { get; set; }

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        public decimal Total => DetallesGasto?.Sum(d => d.Monto) ?? 0;
    }

    public class DetalleGastoCreateEditViewModel
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Debe seleccionar un tipo de gasto.")]
        [Display(Name = "Tipo de Gasto")]
        public int TipoGastoId { get; set; }

        [Required(ErrorMessage = "Debe ingresar un monto.")]
        [Range(0.01, 9999999.99, ErrorMessage = "El monto debe ser entre 0.01 y 9,999,999.99.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }
    }
}