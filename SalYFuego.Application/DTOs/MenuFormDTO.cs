using System.ComponentModel.DataAnnotations;

namespace Sal_Fuego.Aplication.DTOs
{
    public record MenuFormDTO
    {
        public int IdMenu { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeOnly HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        public TimeOnly HoraFin { get; set; }

        public bool EstaActivo { get; set; } = true;

        // Tipo de disponibilidad: "dias" o "fechas"
        public string TipoDisponibilidad { get; set; } = "dias";

        // Para disponibilidad por días de semana
        public List<string> DiasSeleccionados { get; set; } = new();

        // Para disponibilidad por rango de fechas
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }

        // Productos seleccionados
        public List<int> ProductosSeleccionados { get; set; } = new();

        // Combos seleccionados
        public List<int> CombosSeleccionados { get; set; } = new();
    }
}