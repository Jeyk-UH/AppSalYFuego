using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    // Cola de Salón: pedidos Preparados (a empacar, sea para retiro en mostrador o
    // para entregar a un repartidor) y Listos para Retirar (a entregar al cliente
    // en sala), con opción de avanzar al siguiente estado.
    [Authorize(Roles = Roles.Salonero)]
    public class SaloneroController : Controller
    {
        private readonly IServicePedido _servicePedido;

        public SaloneroController(IServicePedido servicePedido)
        {
            _servicePedido = servicePedido;
        }

        public async Task<IActionResult> Index()
        {
            var cola = await _servicePedido.ObtenerColaSalonAsync();
            return View(cola);
        }

        // Refresca la cola sin recargar la página (AJAX)
        [HttpGet]
        public async Task<IActionResult> ObtenerCola()
        {
            var cola = await _servicePedido.ObtenerColaSalonAsync();
            return Json(cola);
        }

        // Avanza un pedido al siguiente estado de su secuencia (empacar / entregar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avanzar([FromBody] int idPedido)
        {
            try
            {
                var idUsuarioTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(idUsuarioTexto, out int idUsuario))
                    return Json(new { exito = false, mensaje = "No se pudo identificar al usuario." });

                var nuevoEstado = await _servicePedido.AvanzarEstadoAsync(idPedido, idUsuario);
                return Json(new { exito = true, nuevoEstado });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }
    }
}
