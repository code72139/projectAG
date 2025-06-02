using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_AG.Models
{

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [Column("Contrasena")]  
    public string Contrasena { get; set; } = string.Empty;
    }

}