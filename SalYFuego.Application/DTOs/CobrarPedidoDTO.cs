namespace Sal_Fuego.Aplication.DTOs
{
    // Datos que envía Encargado/Administrador para cobrar un pedido que quedó
    // Pendiente de Pago (por ejemplo, un pedido del Cliente que eligió pagar en
    // efectivo al retirar/recibir). Ver ServicePedido.CobrarPedidoPendienteAsync.
    public record CobrarPedidoDTO
    {
        public int IdPedido { get; set; }
        public int IdMetodoPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string? TipoTarjeta { get; set; }
        public string? UltimosDigitos { get; set; }
    }
}
