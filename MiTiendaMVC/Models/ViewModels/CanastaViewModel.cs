using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace MiTiendaMVC.Models.ViewModels
{
    public class CanastaViewModel
    {
        public int CanastaId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un cliente.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Selecciona al menos un producto.")]
        public int[] ProductosSeleccionados { get; set; } = Array.Empty<int>();

        [BindNever]                      
        public MultiSelectList? ProductosList { get; set; }
    }
}
