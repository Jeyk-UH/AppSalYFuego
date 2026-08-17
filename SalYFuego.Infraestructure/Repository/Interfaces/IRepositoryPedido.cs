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

        // Historial del Cliente logueado, ordenado por fecha descendente
        Task<ICollection<Pedido>> ListarPorClienteAsync(int idCliente);

        // Historial completo para Encargado/Administrador, con filtro opcional por fecha y estado
        Task<ICollection<Pedido>> ListarTodosAsync(DateTime? fecha, int? idEstado);

        // Pedido completo (cliente, empleado, estado, líneas de detalle con producto/combo, pago) para el detalle tipo factura
        Task<Pedido?> FindDetalleByIdAsync(int id);
    }
}
