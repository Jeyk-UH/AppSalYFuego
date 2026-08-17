namespace Sal_Fuego.Aplication.DTOs
{
    public record EstadoPedidoDTO
    {
        public int IdEstado { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
