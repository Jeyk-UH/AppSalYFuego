namespace Sal_Fuego.Aplication.DTOs
{
    public record PasoProcesoDTO
    {
        public int OrdenPaso { get; set; }
        public string NombreEstacion { get; set; } = null!;
        public int TiempoEstimadoMinutos { get; set; }
    }
}