using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Infraestructure.Repository.Interfaces
{
    // Resultado de la consulta de productos más vendidos.
    // No es una entidad de base de datos, solo el resultado de una agregación.
    public class ProductoVendidoResultado
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = null!;
        public int CantidadVendida { get; set; }
    }

    public interface IRepositoryPedido
    {
        // Cantidad total de pedidos registrados
        Task<int> ContarPedidosAsync();

        // Top de productos más vendidos según las cantidades registradas en DetallePedido
        Task<ICollection<ProductoVendidoResultado>> ObtenerProductosMasVendidosAsync(int top);

        // Inserta el pedido junto con sus DetallePedido y Pago (una sola transacción)
        Task<Pedido> CrearAsync(Pedido pedido);
    }
}
