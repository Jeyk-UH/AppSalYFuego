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

    // Resultado de la pantalla de Reportes: un producto o combo vendido, sin
    // distinguir cuál de los dos es (para poder mezclarlos en un mismo ranking).
    public class ItemVendidoResultado
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!; // "Producto" o "Combo"
        public int CantidadVendida { get; set; }
    }

    // Resultado de la pantalla de Reportes: cantidad de pedidos en una fecha puntual.
    public class PedidosPorDiaResultado
    {
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
    }

    public interface IRepositoryPedido
    {
        // Cantidad total de pedidos registrados
        Task<int> ContarPedidosAsync();

        // Top de productos más vendidos según las cantidades registradas en DetallePedido
        Task<ICollection<ProductoVendidoResultado>> ObtenerProductosMasVendidosAsync(int top);

        // Reporte: pedidos registrados entre dos fechas (inclusive), agrupados por día
        // (solo trae los días que sí tuvieron al menos un pedido; completar los días
        // en cero es responsabilidad del Service, que arma el reporte completo).
        Task<ICollection<PedidosPorDiaResultado>> ObtenerPedidosEntreAsync(DateTime desde, DateTime hasta);

        // Reporte: top de productos Y combos más vendidos entre dos fechas, mezclados
        // en un solo ranking
        Task<ICollection<ItemVendidoResultado>> ObtenerMasVendidosAsync(DateTime desde, DateTime hasta, int top);

        // Reporte: los productos y combos con menos ventas entre dos fechas (entre los
        // que sí se han vendido al menos una vez; no incluye los que nunca se han pedido)
        Task<ICollection<ItemVendidoResultado>> ObtenerMenosVendidosAsync(DateTime desde, DateTime hasta, int top);

        // Inserta el pedido junto con sus DetallePedido y Pago (una sola transacción)
        Task<Pedido> CrearAsync(Pedido pedido);

        // Historial del Cliente logueado, ordenado por fecha descendente
        Task<ICollection<Pedido>> ListarPorClienteAsync(int idCliente);

        // Historial completo para Encargado/Administrador, con filtro opcional por fecha,
        // estado, y exclusión de estados (para el filtro "solo activos" que oculta los
        // pedidos en un estado terminal: Entregado o Retirado)
        Task<ICollection<Pedido>> ListarTodosAsync(DateTime? fecha, int? idEstado, int[]? idsEstadoExcluir = null);

        // Pedido completo (cliente, empleado, estado, líneas de detalle con producto/combo, pago) para el detalle tipo factura
        Task<Pedido?> FindDetalleByIdAsync(int id);

        // Pedidos en cualquiera de los estados indicados, ordenados por fecha ascendente
        // (FIFO): el que entró primero es el que se prepara primero. Usado por la cola de Cocina.
        Task<ICollection<Pedido>> ListarPorEstadosAsync(int[] idsEstado);

        // Los "top" pedidos más recientes entre los estados indicados (fecha descendente).
        // Usado para la columna "Finalizado" del tablero: solo los últimos N, no todo el historial.
        Task<ICollection<Pedido>> ListarUltimosPorEstadosAsync(int[] idsEstado, int top);

        // Pedido "liviano" (sin includes), trackeado por EF, listo para cambiarle el estado
        Task<Pedido?> FindParaActualizarEstadoAsync(int id);

        // Persiste los cambios hechos sobre un Pedido ya trackeado (ver FindParaActualizarEstadoAsync)
        Task GuardarCambiosEstadoAsync();
    }
}
