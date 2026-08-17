namespace Sal_Fuego.Aplication.DTOs
{
    // Datos que envía Caja para registrar una venta
    public record VentaCrearDTO
    {
        // Cliente registrado (opcional). Si es null, se usa NombreClienteInvitado/CedulaClienteInvitado.
        public int? IdCliente { get; set; }
        public string? NombreClienteInvitado { get; set; }
        public string? CedulaClienteInvitado { get; set; }

        // El cajero marcó "Cliente registrado" en pantalla: si viene en true, el
        // servidor exige que IdCliente venga poblado (no se permite degradar
        // silenciosamente a venta anónima). Se valida también en el navegador,
        // pero se repite aquí por si el request no viene de la pantalla de Caja.
        public bool RequiereClienteRegistrado { get; set; }

        // "Recogida en tienda" o "Entrega a domicilio"
        public string MetodoEntrega { get; set; } = "Recogida en tienda";

        // Solo obligatorios si MetodoEntrega es "Entrega a domicilio" (y solo
        // aplica con cliente registrado, ver ServicePedido.CrearVentaAsync)
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string? DireccionExacta { get; set; }
        public string? Referencia { get; set; }

        public int IdMetodoPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string? TipoTarjeta { get; set; }
        public string? UltimosDigitos { get; set; }

        public List<VentaItemDTO> Items { get; set; } = new();
    }
}
