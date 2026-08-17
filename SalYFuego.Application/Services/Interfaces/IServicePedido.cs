using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServicePedido
    {
        // Estadísticas para el panel de administración:
        // cantidad de pedidos realizados y productos más vendidos
        Task<EstadisticasAdminDTO> ObtenerEstadisticasAdminAsync();

        // Catálogo para Caja: productos y combos del menú disponible ahora
        Task<ICollection<MenuItemDTO>> ObtenerCatalogoVentaAsync();

        // Registra una venta hecha por Caja (o Administrador). Revalida precios
        // contra la base de datos, calcula impuesto/total y guarda Pedido+Detalle+Pago.
        Task<VentaResultadoDTO> CrearVentaAsync(VentaCrearDTO dto, int idEmpleado);

        // Registra un pedido hecho por el propio Cliente desde el carrito público.
        // A diferencia de Caja, el IdCliente es obligatorio (no hay pedidos anónimos
        // por este camino) y el pedido queda "Pendiente de Pago" hasta que lo gestione
        // el Encargado/Caja.
        Task<VentaResultadoDTO> CrearPedidoClienteAsync(PedidoClienteCrearDTO dto, int idCliente);

        // Historial de pedidos del Cliente logueado
        Task<ICollection<PedidoListaDTO>> ObtenerHistorialClienteAsync(int idCliente);

        // Historial completo para Encargado/Administrador, con filtro opcional por fecha,
        // estado, y "solo activos" (oculta los pedidos ya Entregados)
        Task<ICollection<PedidoListaDTO>> ObtenerHistorialTodosAsync(DateTime? fecha, int? idEstado, bool soloActivos = false);

        // Detalle de un pedido en formato de factura
        Task<PedidoDetalleDTO?> ObtenerDetalleAsync(int idPedido);

        // Cola de Cocina: pedidos Aceptados o en Preparación, del más antiguo al más
        // reciente (FIFO)
        Task<ICollection<PedidoColaDTO>> ObtenerColaCocinaAsync();

        // Avanza el pedido al siguiente estado de la secuencia (usado por Cocina).
        // Devuelve el nuevo IdEstado.
        Task<int> AvanzarEstadoAsync(int idPedido, int idUsuario);

        // Cambia el pedido a un estado específico (usado por Encargado/Administrador
        // desde el Detalle de Pedido, para corregir o gestionar manualmente)
        Task CambiarEstadoAsync(int idPedido, int idEstadoNuevo, int idUsuario);

        // Cobra un pedido Pendiente de Pago (por ejemplo, uno del Cliente pagado en
        // efectivo al retirar/recibir). Crea el Pago y avanza el pedido a Pagado.
        Task<VentaResultadoDTO> CobrarPedidoPendienteAsync(CobrarPedidoDTO dto, int idUsuario);
    }
}
