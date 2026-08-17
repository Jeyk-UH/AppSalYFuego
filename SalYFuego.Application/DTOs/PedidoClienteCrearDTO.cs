namespace Sal_Fuego.Aplication.DTOs
{
    // Datos que envía el Cliente al confirmar su propio pedido desde el carrito.
    // El cliente siempre queda identificado (IdCliente se toma de la sesión, no viaja en el DTO).
    public record PedidoClienteCrearDTO
    {
        // "Recogida en tienda" o "Entrega a domicilio"
        public string MetodoEntrega { get; set; } = "Recogida en tienda";

        // Solo obligatorios si MetodoEntrega es "Entrega a domicilio"
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string? DireccionExacta { get; set; }
        public string? Referencia { get; set; }

        public int IdMetodoPago { get; set; }

        // Solo vienen si el método elegido es tarjeta: en ese caso se simula el
        // cobro inmediato en línea. Con efectivo se paga en persona al retirar o
        // recibir el pedido (lo cobra Encargado luego, ver ServicePedido.CobrarPedidoPendienteAsync).
        public string? TipoTarjeta { get; set; }
        public string? UltimosDigitos { get; set; }

        public List<VentaItemDTO> Items { get; set; } = new();
    }
}
