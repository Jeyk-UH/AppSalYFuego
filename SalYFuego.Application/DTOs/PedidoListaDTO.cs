namespace Sal_Fuego.Aplication.DTOs
{
    // Fila del historial de pedidos (Cliente ve el suyo; Encargado/Administrador ven todos)
    public record PedidoListaDTO
    {
        public int IdPedido { get; set; }
        public string CodigoOrden { get; set; } = null!;
        public DateTime FechaPedido { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public string EstadoNombre { get; set; } = null!;
        public decimal Total { get; set; }
    }
}
