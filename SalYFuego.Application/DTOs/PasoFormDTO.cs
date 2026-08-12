using System.ComponentModel.DataAnnotations;

namespace Sal_Fuego.Aplication.DTOs
{
    public record PasoFormDTO
    {
        public int OrdenPaso { get; set; }

        [Required(ErrorMessage = "La estación es obligatoria")]
        public int IdEstacion { get; set; }

        public string? NombreEstacion { get; set; }

        [Required(ErrorMessage = "El tiempo es obligatorio")]
        [Range(1, 999, ErrorMessage = "El tiempo debe ser mayor a 0")]
        public int TiempoEstimadoMinutos { get; set; }
    }
}