// Models/Producto.cs
using System.ComponentModel.DataAnnotations;

namespace MiTiendaMVC.Models;

public class Producto
{
    public int ProductoId { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Currency)]
    public decimal Precio { get; set; }

    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    public int Stock { get; set; }
}
