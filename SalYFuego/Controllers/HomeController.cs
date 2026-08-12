using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
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
            // Cada rol operativo tiene su propia pantalla de inicio.
            // Anónimos, Cliente y Administrador ven el inicio público con productos destacados.
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(Roles.Encargado))
                    return RedirectToAction("Index", "Caja");

                if (User.IsInRole(Roles.Cocina))
                    return RedirectToAction("Index", "Cocina");

                if (User.IsInRole(Roles.Salonero))
                    return RedirectToAction("Index", "Salonero");

                if (User.IsInRole(Roles.Repartidor))
                    return RedirectToAction("Index", "Repartidor");
            }

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