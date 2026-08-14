using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServicePedido
    {
        // Estadísticas para el panel de administración:
        // cantidad de pedidos realizados y productos más vendidos
        Task<EstadisticasAdminDTO> ObtenerEstadisticasAdminAsync();
    }
}
