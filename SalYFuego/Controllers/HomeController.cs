using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceProducto _serviceProducto;

        public HomeController(IServiceProducto serviceProducto)
        {
            _serviceProducto = serviceProducto;
        }

        public async Task<IActionResult> Index()
        {
            // Traer todos los productos para mostrar uno por categoría destacada
            var productos = await _serviceProducto.ListAsync();

            // Seleccionar un producto representativo de cada categoría que queremos destacar
            var destacados = productos
                .Where(p => new[] { "Tacos", "Comida Rápida", "Corte de Carnes", "Bebidas Naturales" }
                    .Contains(p.CategoriaNombre))
                .GroupBy(p => p.CategoriaNombre)
                .Select(g => g.First())
                .ToList();

            return View(destacados);
        }
    }
}