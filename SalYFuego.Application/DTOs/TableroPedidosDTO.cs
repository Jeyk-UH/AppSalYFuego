namespace Sal_Fuego.Aplication.DTOs
{
    // Pedidos agrupados para el tablero de Encargado/Administrador: los activos
    // (cualquier estado no terminal) más los últimos N ya finalizados (Entregado
    // o Retirado), para no cargar todo el historial completo en pantalla.
    public record TableroPedidosDTO
    {
        public List<PedidoListaDTO> Activos { get; set; } = new();
        public List<PedidoListaDTO> Finalizados { get; set; } = new();
    }
}
