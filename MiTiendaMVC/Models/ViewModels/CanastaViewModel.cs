using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;   // ← Este using es obligatorio

namespace MiTiendaMVC.Models.ViewModels
{
    public class CanastaViewModel
    {
        public int CanastaId { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Display(Name = "Productos")]
        public int[] ProductosSeleccionados { get; set; } = new int[0];

        // Ahora MultiSelectList se reconoce
        public MultiSelectList ProductosList { get; set; } = null!;
    }
}
