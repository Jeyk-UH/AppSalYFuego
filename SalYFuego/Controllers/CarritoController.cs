using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    // Carrito de compra público del Cliente: navega el menú disponible, arma su
    // pedido y lo confirma. A diferencia de Caja, aquí el cliente siempre queda
    // identificado (no hay pedidos anónimos por este camino).
    [Authorize(Roles = Roles.Cliente)]
    public class CarritoController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServiceMetodoPago _serviceMetodoPago;
        private readonly IServiceClima _serviceClima;

        public CarritoController(
            IServicePedido servicePedido,
            IServiceMetodoPago serviceMetodoPago,
            IServiceClima serviceClima)
        {
            _servicePedido = servicePedido;
            _serviceMetodoPago = serviceMetodoPago;
            _serviceClima = serviceClima;
        }

        // Pantalla del carrito: catálogo del menú disponible ahora + métodos de pago activos.
        // Consumo del Web Service externo Open-Meteo: si no responde, ViewBag.Clima
        // queda en null y la vista simplemente no muestra el widget (no rompe la página).
        public async Task<IActionResult> Index()
        {
            var catalogo = await _servicePedido.ObtenerCatalogoVentaAsync();
            // Checkout en línea: Efectivo (paga al retirar/recibir) o Pago Web
            // (cobro simulado en línea con tarjeta). No aplica pagar con tarjeta
            // "física" acá, eso es solo para Caja/Cobrar en persona.
            ViewBag.MetodosPago = await _serviceMetodoPago.ListEnLineaAsync();
            ViewBag.CostoEnvioDomicilio = TarifasEnvio.CostoDomicilio;
            ViewBag.Clima = await _serviceClima.ObtenerClimaActualAsync();
            return View(catalogo);
        }

        // Confirmar el pedido y devolver el resumen (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comprar([FromBody] PedidoClienteCrearDTO dto)
        {
            if (dto?.Items == null || dto.Items.Count == 0)
                return Json(new { exito = false, mensaje = "El carrito está vacío." });

            try
            {
                var idClienteTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(idClienteTexto, out int idCliente))
                    return Json(new { exito = false, mensaje = "No se pudo identificar al cliente." });

                var resultado = await _servicePedido.CrearPedidoClienteAsync(dto, idCliente);
                return Json(new { exito = true, resultado });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }
    }
}
