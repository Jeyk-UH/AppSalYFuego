namespace Sal_Fuego.Aplication.DTOs
{
    // Detalle completo del pedido en formato de factura
    public record PedidoDetalleDTO
    {
        public int IdPedido { get; set; }
        // Usado internamente para validar que el Cliente solo vea sus propios pedidos
        public int? IdCliente { get; set; }

        public string CodigoOrden { get; set; } = null!;
        public DateTime FechaPedido { get; set; }

        // Encabezado
        public string ClienteNombre { get; set; } = null!;
        public string ClienteIdentificador { get; set; } = null!; // correo o cédula, según el caso
        public string EncargadoNombre { get; set; } = null!;
        public string MetodoEntrega { get; set; } = null!;
        // Solo tiene valor cuando MetodoEntrega es "Entrega a domicilio"
        public string? DireccionEntrega { get; set; }
        public string MetodoPagoNombre { get; set; } = null!;
        public string EstadoNombre { get; set; } = null!;

        // Detalle
        public List<DetalleLineaDTO> Lineas { get; set; } = new();

        // Totales
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Total { get; set; }
    }
}
