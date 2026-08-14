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
    }
}
