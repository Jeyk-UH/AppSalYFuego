namespace Sal_Fuego.Aplication.DTOs
{
    public record MetodoPagoDTO
    {
        public int IdMetodoPago { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
