using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    // Cola de Repartidor: pedidos En Espera Repartidor (a recoger) y En Ruta (a
    // marcar entregado), con opción de avanzar al siguiente estado. Es una cola
    // compartida entre todos los repartidores: la base de datos no asigna un
    // pedido a un repartidor específico.
    [Authorize(Roles = Roles.Repartidor)]
    public class RepartidorController : Controller
    {
        private readonly IServicePedido _servicePedido;

        public RepartidorController(IServicePedido servicePedido)
        {
            _servicePedido = servicePedido;
        }

        public async Task<IActionResult> Index()
        {
            var cola = await _servicePedido.ObtenerColaRepartidorAsync();
            return View(cola);
        }

        // Refresca la cola sin recargar la página (AJAX)
        [HttpGet]
        public async Task<IActionResult> ObtenerCola()
        {
            var cola = await _servicePedido.ObtenerColaRepartidorAsync();
            return Json(cola);
        }

        // Avanza un pedido al siguiente estado de su secuencia (salir a repartir / entregado)
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
