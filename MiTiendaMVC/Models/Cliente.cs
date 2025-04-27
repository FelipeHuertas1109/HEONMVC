// Models/Cliente.cs
using System.ComponentModel.DataAnnotations;

namespace MiTiendaMVC.Models;

public class Cliente
{
    public int ClienteId { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string Telefono { get; set; } = string.Empty;

    // 🔑 Propiedad de navegación a la única canasta
    public Canasta? Canasta { get; set; }
}
