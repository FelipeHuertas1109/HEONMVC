// Models/Canasta.cs
using System.Collections.Generic;

namespace MiTiendaMVC.Models;

public class Canasta
{
    public int CanastaId { get; set; }

    // 🔑 Cliente asociado (FK)
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    // Relación productos en la canasta
    public List<Producto> Productos { get; set; } = new();

    public int Total { get; set; }
}
