namespace Sal_Fuego.Aplication.DTOs
{
    // Pedido tal como lo necesita ver Cocina: sin precios, solo lo necesario
    // para preparar la comanda. Se usa en la cola FIFO de Cocina.
    public record PedidoColaDTO
    {
        public int IdPedido { get; set; }
        public string CodigoOrden { get; set; } = null!;
        public DateTime FechaPedido { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public int IdEstado { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public string MetodoEntrega { get; set; } = null!;
        // Solo viene poblado si MetodoEntrega es "Entrega a domicilio" (lo necesita Repartidor)
        public string? DireccionEntrega { get; set; }
        public List<DetalleLineaColaDTO> Lineas { get; set; } = new();
    }

    public record DetalleLineaColaDTO
    {
        public string Nombre { get; set; } = null!;
        public int Cantidad { get; set; }
        public string? Observaciones { get; set; }
    }
}
