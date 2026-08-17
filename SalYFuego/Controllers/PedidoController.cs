using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    // Historial y detalle de pedidos. Cliente ve solo lo suyo;
    // Encargado y Administrador ven todos, con filtros por fecha y estado.
    [Authorize(Roles = Roles.Cliente + "," + Roles.Encargado + "," + Roles.Administrador)]
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServiceEstadoPedido _serviceEstadoPedido;

        public PedidoController(IServicePedido servicePedido, IServiceEstadoPedido serviceEstadoPedido)
        {
            _servicePedido = servicePedido;
            _serviceEstadoPedido = serviceEstadoPedido;
        }

        // Historial: el comportamiento depende del rol del usuario logueado
        public async Task<IActionResult> Index(DateTime? fecha, int? idEstado)
        {
            bool esStaff = EsStaff();
            ViewBag.EsStaff = esStaff;

            if (esStaff)
            {
                ViewBag.Estados = await _serviceEstadoPedido.ListAsync();
                ViewBag.FechaSeleccionada = fecha?.ToString("yyyy-MM-dd");
                ViewBag.EstadoSeleccionado = idEstado;

                var pedidos = await _servicePedido.ObtenerHistorialTodosAsync(fecha, idEstado);
                return View(pedidos);
            }

            var idCliente = ObtenerIdUsuarioActual();
            var propios = await _servicePedido.ObtenerHistorialClienteAsync(idCliente);
            return View(propios);
        }

        // Igual que Index, pero devuelve JSON para refrescar la tabla sin recargar la página
        // (solo lo usan Encargado/Administrador desde los filtros)
        [HttpGet]
        public async Task<IActionResult> Filtrar(DateTime? fecha, int? idEstado)
        {
            if (!EsStaff())
                return Forbid();

            var pedidos = await _servicePedido.ObtenerHistorialTodosAsync(fecha, idEstado);
            return Json(pedidos);
        }

        // Detalle de un pedido en formato de factura
        public async Task<IActionResult> Detalle(int id)
        {
            var detalle = await _servicePedido.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            // Un Cliente solo puede ver sus propios pedidos
            if (!EsStaff() && detalle.IdCliente != ObtenerIdUsuarioActual())
                return Forbid();

            return View(detalle);
        }

        private bool EsStaff() =>
            User.IsInRole(Roles.Encargado) || User.IsInRole(Roles.Administrador);

        private int ObtenerIdUsuarioActual() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
