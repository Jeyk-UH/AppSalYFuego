using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sal_Fuego.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    public class AdminComboController : Controller
    {
        private const int TamanoPagina = 10;

        private readonly IServiceCombo _serviceCombo;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceProducto _serviceProducto;
        private readonly IWebHostEnvironment _env;

        public AdminComboController(
            IServiceCombo serviceCombo,
            IServiceCategoria serviceCategoria,
            IServiceProducto serviceProducto,
            IWebHostEnvironment env)
        {
            _serviceCombo = serviceCombo;
            _serviceCategoria = serviceCategoria;
            _serviceProducto = serviceProducto;
            _env = env;
        }

        // Listado con filtro por categoría y paginación
        public async Task<IActionResult> Index(int? categoriaId, string? busqueda, int? page)
        {
            var combos = await _serviceCombo.ListAsync();
            var categorias = await _serviceCategoria.ListAsync();

            if (categoriaId.HasValue)
                combos = combos
                    .Where(c => c.IdCategoria == categoriaId)
                    .ToList();

            if (!string.IsNullOrEmpty(busqueda))
                combos = combos
                    .Where(c => c.Nombre.Contains(busqueda,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

            ViewBag.Categorias = new SelectList(
                categorias, "IdCategoria", "Nombre", categoriaId);
            ViewBag.CategoriaSeleccionada = categoriaId;
            ViewBag.Busqueda = busqueda;

            var paginado = combos.ToPagedList(page ?? 1, TamanoPagina);
            return View(paginado);
        }

        // Formulario agregar
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            await CargarViewBags();
            return View(new ComboFormDTO());
        }

        // Guardar nuevo combo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(ComboFormDTO dto)
        {
            // Temporal: ver qué campos fallan la validación
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new {
                        x.Key,
                        Errores = x.Value.Errors.Select(e => e.ErrorMessage)
                    });

                foreach (var error in errores)
                    TempData["Error"] = $"{error.Key}: {string.Join(", ", error.Errores)}";

                await CargarViewBags();
                return View(dto);
            }

            await _serviceCombo.AddAsync(dto, _env.WebRootPath);
            TempData["Exito"] = "Combo agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Formulario editar
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _serviceCombo.FindFormByIdAsync(id);
            if (dto == null) return NotFound();

            await CargarViewBags();
            return View(dto);
        }

        // Guardar cambios del combo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ComboFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBags();
                return View(dto);
            }

            await _serviceCombo.UpdateAsync(dto, _env.WebRootPath);
            TempData["Exito"] = "Combo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Desactivar combo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _serviceCombo.DesactivarAsync(id);
            TempData["Exito"] = "Combo desactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Cargar categorías y productos para los formularios
        private async Task CargarViewBags()
        {
            var categorias = await _serviceCategoria.ListAsync();
            var productos = await _serviceProducto.ListAsync();

            ViewBag.Categorias = new SelectList(
                categorias, "IdCategoria", "Nombre");
            ViewBag.TodosProductos = productos
                .Where(p => p.Activo)
                .ToList();
        }
    }
}