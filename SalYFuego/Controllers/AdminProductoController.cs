using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    public class AdminProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceIngrediente _serviceIngrediente;
        private readonly IWebHostEnvironment _env;

        public AdminProductoController(
            IServiceProducto serviceProducto,
            IServiceCategoria serviceCategoria,
            IServiceIngrediente serviceIngrediente,
            IWebHostEnvironment env)
        {
            _serviceProducto = serviceProducto;
            _serviceCategoria = serviceCategoria;
            _serviceIngrediente = serviceIngrediente;
            _env = env;
        }

        // Listado con filtro por categoría y búsqueda por nombre
        public async Task<IActionResult> Index(int? categoriaId, string? busqueda)
        {
            var productos = await _serviceProducto.ListAsync();
            var categorias = await _serviceCategoria.ListAsync();

            if (categoriaId.HasValue)
                productos = productos
                    .Where(p => p.IdCategoria == categoriaId)
                    .ToList();

            if (!string.IsNullOrEmpty(busqueda))
                productos = productos
                    .Where(p => p.Nombre.Contains(busqueda,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre", categoriaId);
            ViewBag.CategoriaSeleccionada = categoriaId;
            ViewBag.Busqueda = busqueda;

            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            await CargarViewBags();
            return View(new ProductoFormDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(ProductoFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBags(dto.IngredientesSeleccionados);
                return View(dto);
            }

            await _serviceProducto.AddAsync(dto, _env.WebRootPath);
            TempData["Exito"] = "Producto agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _serviceProducto.FindFormByIdAsync(id);
            if (dto == null) return NotFound();

            await CargarViewBags(dto.IngredientesSeleccionados);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ProductoFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBags(dto.IngredientesSeleccionados);
                return View(dto);
            }

            await _serviceProducto.UpdateAsync(dto, _env.WebRootPath);
            TempData["Exito"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _serviceProducto.DesactivarAsync(id);
            TempData["Exito"] = "Estado del producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarViewBags(List<int>? seleccionados = null)
        {
            var categorias = await _serviceCategoria.ListAsync();
            var ingredientes = await _serviceIngrediente.ListAsync();

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre");
            ViewBag.TodosIngredientes = ingredientes;
            ViewBag.IngredientesSeleccionados = seleccionados ?? new List<int>();
        }
    }
}