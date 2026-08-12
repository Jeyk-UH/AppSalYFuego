namespace Sal_Fuego.Aplication.DTOs
{
    public record EstacionDTO
    {
        public int IdEstacion { get; set; }
        public string Nombre { get; set; } = null!;
    }
}