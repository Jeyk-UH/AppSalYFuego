namespace Sal_Fuego.Aplication.DTOs
{
    // Datos que envía Encargado/Administrador para cambiar manualmente el estado
    // de un pedido desde el Detalle de Pedido
    public record CambiarEstadoDTO
    {
        public int IdPedido { get; set; }
        public int IdEstadoNuevo { get; set; }
    }
}
