using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Web.Controllers
{
    public class ProductoController : Controller
    {
        // Inyectamos nuestro servicio de productos
        private readonly IServiceProducto _service;

        public ProductoController(IServiceProducto service)
        {
            _service = service;
        }

        // Acción para listar los productos
        public async Task<IActionResult> Index()
        {
            var productos = await _service.ListAsync();
            return View(productos); // Pasa la lista de DTOs a la vista Index.cshtml
        }

        // Acción para el detalle de un producto
        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _service.FindByIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto); // Pasa el DTO específico a la vista Detalle.cshtml
        }
    }
}