using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    // Punto de venta del Encargado (cajero). El Administrador también puede
    // acceder desde el botón "Realizar Venta" de su panel.
    [Authorize(Roles = Roles.Encargado + "," + Roles.Administrador)]
    public class CajaController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceMetodoPago _serviceMetodoPago;

        public CajaController(
            IServicePedido servicePedido,
            IServiceUsuario serviceUsuario,
            IServiceMetodoPago serviceMetodoPago)
        {
            _servicePedido = servicePedido;
            _serviceUsuario = serviceUsuario;
            _serviceMetodoPago = serviceMetodoPago;
        }

        // Pantalla de venta: catálogo del menú disponible ahora + métodos de pago activos
        public async Task<IActionResult> Index()
        {
            var catalogo = await _servicePedido.ObtenerCatalogoVentaAsync();
            ViewBag.MetodosPago = await _serviceMetodoPago.ListAsync();
            return View(catalogo);
        }

        // Búsqueda de clientes registrados por nombre, correo o cédula (AJAX)
        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string termino)
        {
            var clientes = await _serviceUsuario.BuscarClientesAsync(termino);
            return Json(clientes);
        }

        // Registrar la venta y devolver el recibo (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cobrar([FromBody] VentaCrearDTO dto)
        {
            if (dto?.Items == null || dto.Items.Count == 0)
                return Json(new { exito = false, mensaje = "El carrito está vacío." });

            try
            {
                var idUsuarioTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(idUsuarioTexto, out int idEmpleado))
                    return Json(new { exito = false, mensaje = "No se pudo identificar al usuario que realiza la venta." });

                var resultado = await _servicePedido.CrearVentaAsync(dto, idEmpleado);
                return Json(new { exito = true, resultado });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }
    }
}
