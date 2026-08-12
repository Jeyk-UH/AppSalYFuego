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
    public class AdminMenuController : Controller
    {
        private const int TamanoPagina = 10;

        private readonly IServiceMenu _serviceMenu;
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCombo _serviceCombo;

        public AdminMenuController(
            IServiceMenu serviceMenu,
            IServiceProducto serviceProducto,
            IServiceCombo serviceCombo)
        {
            _serviceMenu = serviceMenu;
            _serviceProducto = serviceProducto;
            _serviceCombo = serviceCombo;
        }

        // Listado de menús, paginado
        public async Task<IActionResult> Index(int? page)
        {
            var menus = await _serviceMenu.ListAsync();
            var paginado = menus.ToPagedList(page ?? 1, TamanoPagina);
            return View(paginado);
        }

        // Formulario agregar
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            await CargarViewBags();
            return View(new MenuFormDTO());
        }

        // Guardar nuevo menú
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(MenuFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBags();
                return View(dto);
            }

            await _serviceMenu.AddAsync(dto);
            TempData["Exito"] = "Menú agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Formulario editar
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _serviceMenu.FindFormByIdAsync(id);
            if (dto == null) return NotFound();

            await CargarViewBags();
            return View(dto);
        }

        // Guardar cambios del menú
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(MenuFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBags();
                return View(dto);
            }

            await _serviceMenu.UpdateAsync(dto);
            TempData["Exito"] = "Menú actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Desactivar menú
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _serviceMenu.DesactivarAsync(id);
            TempData["Exito"] = "Menú desactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Cargar productos y combos para los formularios
        private async Task CargarViewBags()
        {
            var productos = await _serviceProducto.ListAsync();
            var combos = await _serviceCombo.ListAsync();

            ViewBag.TodosProductos = productos
                .Where(p => p.Activo)
                .ToList();
            ViewBag.TodosCombos = combos
                .Where(c => c.Activo)
                .ToList();
        }
    }
}