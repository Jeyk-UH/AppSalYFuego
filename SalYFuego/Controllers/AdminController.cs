using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    // Panel de inicio del Administrador: estadísticas y accesos rápidos.
    [Authorize(Roles = Roles.Administrador)]
    public class AdminController : Controller
    {
        private readonly IServicePedido _servicePedido;

        public AdminController(IServicePedido servicePedido)
        {
            _servicePedido = servicePedido;
        }

        public async Task<IActionResult> Index()
        {
            var estadisticas = await _servicePedido.ObtenerEstadisticasAdminAsync();
            return View(estadisticas);
        }

        // Pantalla de Reportes: gráficos de pedidos por día, más vendidos y menos vendidos.
        // Desde/Hasta son opcionales (filtro de fechas); si no vienen, el Service usa
        // por defecto la última semana.
        public async Task<IActionResult> Reportes(DateTime? desde, DateTime? hasta)
        {
            var reporte = await _servicePedido.ObtenerReporteVentasAsync(desde, hasta);
            return View(reporte);
        }
    }
}
