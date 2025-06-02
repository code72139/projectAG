using System.ComponentModel.DataAnnotations;

namespace Project_AG.Models
{
    /// <summary>
    /// Representa una categoría o tipo de gasto en el sistema
    /// </summary>
    public class TipoGasto
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}
