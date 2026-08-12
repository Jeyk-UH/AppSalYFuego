using System.ComponentModel.DataAnnotations;

namespace Sal_Fuego.Aplication.DTOs
{
    public record ProcesoFormDTO
    {
        // El IdProducto es obligatorio para asociar el proceso al producto correspondiente
        [Required(ErrorMessage = "El producto es obligatorio")]
        // Se puede agregar un atributo Display para mostrar un nombre más amigable en los mensajes de error
        public int IdProducto { get; set; }

        // Nullable para que no sea requerido en el formulario
        public string? NombreProducto { get; set; }
        // Lista de pasos del proceso, inicializada como una lista vacía

        public List<PasoFormDTO> Pasos { get; set; } = new();
    }
}