using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sal_Fuego.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    public class IngredienteController : Controller
    {
        private const int TamanoPagina = 10;

        private readonly IServiceIngrediente _service;

        public IngredienteController(IServiceIngrediente service) => _service = service;

        // Listado de ingredientes, paginado
        public async Task<IActionResult> Index(int? page)
        {
            var ingredientes = await _service.ListAsync();
            var paginado = ingredientes.ToPagedList(page ?? 1, TamanoPagina);
            return View(paginado);
        }

        // Formulario para agregar
        [HttpGet]
        public IActionResult Agregar()
        {
            return View(new IngredienteDTO());
        }

        // Guardar nuevo ingrediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(IngredienteDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _service.AddAsync(dto);
            TempData["Exito"] = "Ingrediente agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Formulario para editar
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var ingrediente = await _service.FindByIdAsync(id);
            if (ingrediente == null) return NotFound();
            return View(ingrediente);
        }

        // Guardar cambios del ingrediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(IngredienteDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _service.UpdateAsync(dto);
            TempData["Exito"] = "Ingrediente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Eliminar ingrediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Exito"] = "Ingrediente eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}