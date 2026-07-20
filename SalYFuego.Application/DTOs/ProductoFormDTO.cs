using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sal_Fuego.Aplication.DTOs
{
    public record ProductoFormDTO
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(1, 999999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int IdCategoria { get; set; }

        // Ids de ingredientes seleccionados
        public List<int> IngredientesSeleccionados { get; set; } = new();

        // Imagen nueva (opcional al editar)
        public IFormFile? ImagenFile { get; set; }

        // URL de imagen actual (para mostrar al editar)
        public string? ImagenActual { get; set; }
    }
}