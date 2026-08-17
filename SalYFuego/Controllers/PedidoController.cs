using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
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
        private readonly IServiceMetodoPago _serviceMetodoPago;

        public PedidoController(
            IServicePedido servicePedido,
            IServiceEstadoPedido serviceEstadoPedido,
            IServiceMetodoPago serviceMetodoPago)
        {
            _servicePedido = servicePedido;
            _serviceEstadoPedido = serviceEstadoPedido;
            _serviceMetodoPago = serviceMetodoPago;
        }

        // Historial: el comportamiento depende del rol del usuario logueado.
        // Para Encargado/Administrador, "soloActivos" empieza en true para que los
        // pedidos recién hechos no se "pierdan" de vista entre el resto del historial.
        public async Task<IActionResult> Index(DateTime? fecha, int? idEstado, bool soloActivos = true)
        {
            bool esStaff = EsStaff();
            ViewBag.EsStaff = esStaff;

            if (esStaff)
            {
                ViewBag.Estados = await _serviceEstadoPedido.ListAsync();
                ViewBag.FechaSeleccionada = fecha?.ToString("yyyy-MM-dd");
                ViewBag.EstadoSeleccionado = idEstado;
                ViewBag.SoloActivos = soloActivos;

                var pedidos = await _servicePedido.ObtenerHistorialTodosAsync(fecha, idEstado, soloActivos);
                return View(pedidos);
            }

            var idCliente = ObtenerIdUsuarioActual();
            var propios = await _servicePedido.ObtenerHistorialClienteAsync(idCliente);
            return View(propios);
        }

        // Igual que Index, pero devuelve JSON para refrescar la tabla sin recargar la página
        // (solo lo usan Encargado/Administrador desde los filtros)
        [HttpGet]
        public async Task<IActionResult> Filtrar(DateTime? fecha, int? idEstado, bool soloActivos = false)
        {
            if (!EsStaff())
                return Forbid();

            var pedidos = await _servicePedido.ObtenerHistorialTodosAsync(fecha, idEstado, soloActivos);
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

            if (EsStaff())
            {
                ViewBag.Estados = await _serviceEstadoPedido.ListAsync();
                // Cobro presencial: sin "Pago Web", que es exclusivo del checkout en línea.
                ViewBag.MetodosPago = await _serviceMetodoPago.ListPresencialAsync();
            }
            ViewBag.EsStaff = EsStaff();

            return View(detalle);
        }

        // Cobra un pedido Pendiente de Pago (Encargado/Administrador), por ejemplo
        // uno del Cliente que eligió pagar en efectivo al retirar/recibir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cobrar([FromBody] CobrarPedidoDTO dto)
        {
            if (!EsStaff())
                return Forbid();

            try
            {
                var resultado = await _servicePedido.CobrarPedidoPendienteAsync(dto, ObtenerIdUsuarioActual());
                return Json(new { exito = true, resultado });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // Cambiar el estado de un pedido manualmente (Encargado/Administrador)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoDTO dto)
        {
            if (!EsStaff())
                return Forbid();

            try
            {
                await _servicePedido.CambiarEstadoAsync(dto.IdPedido, dto.IdEstadoNuevo, ObtenerIdUsuarioActual());
                return Json(new { exito = true });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        private bool EsStaff() =>
            User.IsInRole(Roles.Encargado) || User.IsInRole(Roles.Administrador);

        private int ObtenerIdUsuarioActual() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
