using System.ComponentModel.DataAnnotations;

namespace Sal_Fuego.Aplication.DTOs
{
    public record ComboFormDTO
    {
        public int IdCombo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio especial es obligatorio")]
        [Range(1, 999999, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioEspecial { get; set; }

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int IdCategoria { get; set; }

        // Productos seleccionados con su cantidad
        public List<ComboProductoFormDTO> ProductosSeleccionados { get; set; } = new();

        // Imagen
        public string? UrlImagenActual { get; set; }
        public Microsoft.AspNetCore.Http.IFormFile? ImagenFile { get; set; }
    }
}