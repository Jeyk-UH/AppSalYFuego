namespace Sal_Fuego.Aplication.DTOs
{
    public record MenuDisponibilidadDTO
    {
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
        public string? DiaSemana { get; set; }
    }
}