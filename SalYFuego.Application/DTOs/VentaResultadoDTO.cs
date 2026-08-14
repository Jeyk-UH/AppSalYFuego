namespace Sal_Fuego.Aplication.DTOs
{
    // Resumen de la venta ya registrada, para mostrar el recibo en pantalla
    public record VentaResultadoDTO
    {
        public int IdPedido { get; set; }
        public string CodigoOrden { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal Vuelto { get; set; }
    }
}
