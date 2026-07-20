using System.ComponentModel.DataAnnotations;

namespace Sal_Fuego.Aplication.DTOs
{
    public record IngredienteDTO
    {
        public int IdIngrediente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres")]
        public string Nombre { get; set; } = null!;
    }
}