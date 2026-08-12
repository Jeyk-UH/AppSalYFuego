using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sal_Fuego.Web.Controllers
{
    public class ProductoController : Controller
    {
        private const int TamanoPagina = 12;

        // Inyectamos nuestro servicio de productos
        private readonly IServiceProducto _service;
        private readonly IServiceMenu _serviceMenu;

        public ProductoController(IServiceProducto service, IServiceMenu serviceMenu)
        {
            _service = service;
            _serviceMenu = serviceMenu;
        }

        // Acción para listar los productos: solo los activos y que estén
        // dentro del menú disponible en este momento (según hora/día actual)
        public async Task<IActionResult> Index(int? page)
        {
            var menuDisponible = await _serviceMenu.GetMenuDisponibleAsync();
            if (menuDisponible == null)
                return View(Enumerable.Empty<Sal_Fuego.Aplication.DTOs.ProductoDTO>()
                    .ToPagedList(page ?? 1, TamanoPagina));

            var idsDisponibles = menuDisponible.Items
                .Where(i => i.Tipo == "Producto" && i.IdProducto.HasValue)
                .Select(i => i.IdProducto!.Value)
                .ToHashSet();

            var productos = await _service.ListAsync();
            var disponibles = productos
                .Where(p => p.Activo && idsDisponibles.Contains(p.IdProducto))
                .ToList();

            return View(disponibles.ToPagedList(page ?? 1, TamanoPagina));
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