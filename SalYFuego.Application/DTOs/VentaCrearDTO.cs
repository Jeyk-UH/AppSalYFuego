namespace Sal_Fuego.Aplication.DTOs
{
    // Datos que envía Caja para registrar una venta
    public record VentaCrearDTO
    {
        // Cliente registrado (opcional). Si es null, se usa NombreClienteInvitado/CedulaClienteInvitado.
        public int? IdCliente { get; set; }
        public string? NombreClienteInvitado { get; set; }
        public string? CedulaClienteInvitado { get; set; }

        // "Local" o "Para llevar"
        public string MetodoEntrega { get; set; } = "Local";

        public int IdMetodoPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string? TipoTarjeta { get; set; }
        public string? UltimosDigitos { get; set; }

        public List<VentaItemDTO> Items { get; set; } = new();
    }
}
