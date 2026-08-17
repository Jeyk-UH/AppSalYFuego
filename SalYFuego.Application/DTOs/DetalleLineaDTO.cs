namespace Sal_Fuego.Aplication.DTOs
{
    // Una línea de detalle dentro de la factura de un pedido
    public record DetalleLineaDTO
    {
        public string Nombre { get; set; } = null!;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        // Calculado a partir del Subtotal de la línea (no se guarda en BD)
        public decimal Impuesto { get; set; }
        public string? Observaciones { get; set; }
    }
}
