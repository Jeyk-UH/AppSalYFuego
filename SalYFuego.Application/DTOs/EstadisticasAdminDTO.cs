namespace Sal_Fuego.Aplication.DTOs
{
    public record EstadisticasAdminDTO
    {
        // Cantidad total de pedidos registrados
        public int CantidadPedidos { get; set; }
        // Top de productos más vendidos
        public List<ProductoVendidoDTO> ProductosMasVendidos { get; set; } = new();
    }
}
