namespace Sal_Fuego.Aplication.DTOs
{
    // Un renglón del carrito armado en Caja
    public record VentaItemDTO
    {
        // "Producto" o "Combo"
        public string Tipo { get; set; } = null!;
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public string? Observaciones { get; set; }
    }
}
