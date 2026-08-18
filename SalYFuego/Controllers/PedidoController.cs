using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    // Historial y detalle de pedidos. Cliente ve solo lo suyo; Encargado y Administrador
    // ven todos, con filtros por fecha y estado. Salonero y Repartidor pueden entrar al
    // Detalle de cualquier pedido (por ejemplo, desde el link de sus propias colas) para
    // ver la línea de tiempo y la dirección de entrega, pero sin controles de gestión.
    [Authorize(Roles = Roles.Cliente + "," + Roles.Encargado + "," + Roles.Administrador + "," + Roles.Salonero + "," + Roles.Repartidor)]
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
        // Encargado/Administrador ven el tablero por estado (vista principal);
        // el Cliente ve su propio historial simple.
        public async Task<IActionResult> Index()
        {
            bool esStaff = EsStaff();
            ViewBag.EsStaff = esStaff;

            if (esStaff)
            {
                ViewBag.Estados = await _serviceEstadoPedido.ListAsync();
                var tablero = await _servicePedido.ObtenerTableroAsync();
                return View("Tablero", tablero);
            }

            var idCliente = ObtenerIdUsuarioActual();
            var propios = await _servicePedido.ObtenerHistorialClienteAsync(idCliente);
            return View(propios);
        }

        // Lista con filtros de fecha/estado/solo activos (Encargado/Administrador), para
        // buscar un pedido puntual, incluidos los ya finalizados. Accesible desde el tablero.
        public async Task<IActionResult> Lista(DateTime? fecha, int? idEstado, bool soloActivos = true)
        {
            if (!EsStaff())
                return Forbid();

            ViewBag.EsStaff = true;
            ViewBag.Estados = await _serviceEstadoPedido.ListAsync();
            ViewBag.FechaSeleccionada = fecha?.ToString("yyyy-MM-dd");
            ViewBag.EstadoSeleccionado = idEstado;
            ViewBag.SoloActivos = soloActivos;

            var pedidos = await _servicePedido.ObtenerHistorialTodosAsync(fecha, idEstado, soloActivos);
            return View(pedidos);
        }

        // Igual que Lista, pero devuelve JSON para refrescar la tabla sin recargar la página
        // (solo lo usan Encargado/Administrador desde los filtros)
        [HttpGet]
        public async Task<IActionResult> Filtrar(DateTime? fecha, int? idEstado, bool soloActivos = false)
        {
            if (!EsStaff())
                return Forbid();

            var pedidos = await _servicePedido.ObtenerHistorialTodosAsync(fecha, idEstado, soloActivos);
            return Json(pedidos);
        }

        // Refresca el tablero sin recargar la página (tras cambiar el estado de un pedido)
        [HttpGet]
        public async Task<IActionResult> ObtenerTablero()
        {
            if (!EsStaff())
                return Forbid();

            var tablero = await _servicePedido.ObtenerTableroAsync();
            return Json(tablero);
        }

        // Detalle de un pedido en formato de factura
        public async Task<IActionResult> Detalle(int id)
        {
            var detalle = await _servicePedido.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            // Un Cliente solo puede ver sus propios pedidos; el resto de los roles
            // operativos (Encargado, Administrador, Salonero, Repartidor) puede ver
            // el detalle de cualquier pedido.
            if (!PuedeVerCualquierPedido() && detalle.IdCliente != ObtenerIdUsuarioActual())
                return Forbid();

            if (EsStaff())
            {
                ViewBag.Estados = await _serviceEstadoPedido.ListAsync();
                // Cobro presencial: sin "Pago Web", que es exclusivo del checkout en línea.
                ViewBag.MetodosPago = await _serviceMetodoPago.ListPresencialAsync();
            }
            ViewBag.EsStaff = EsStaff();

            // El botón "Volver" debe llevar a la pantalla propia de cada rol, no
            // siempre a Pedido/Index (que para Salonero/Repartidor no aplica).
            if (User.IsInRole(Roles.Salonero))
                ViewBag.VolverUrl = "/Salonero/Index";
            else if (User.IsInRole(Roles.Repartidor))
                ViewBag.VolverUrl = "/Repartidor/Index";
            else
                ViewBag.VolverUrl = "/Pedido/Index";

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

        // Puede ver el Detalle de cualquier pedido (no solo el suyo): además de
        // Encargado/Administrador, también Salonero y Repartidor lo necesitan para
        // ver la dirección de entrega y la línea de tiempo desde sus propias colas.
        private bool PuedeVerCualquierPedido() =>
            EsStaff() || User.IsInRole(Roles.Salonero) || User.IsInRole(Roles.Repartidor);

        private int ObtenerIdUsuarioActual() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
