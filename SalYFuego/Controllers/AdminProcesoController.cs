using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    public class AdminProcesoController : Controller
    {
        private readonly IServiceProceso _serviceProceso;
        private readonly IServiceProducto _serviceProducto;

        public AdminProcesoController(
            IServiceProceso serviceProceso,
            IServiceProducto serviceProducto)
        {
            _serviceProceso = serviceProceso;
            _serviceProducto = serviceProducto;
        }

        // Listado de procesos
        public async Task<IActionResult> Index()
        {
            var procesos = await _serviceProceso.ListAsync();
            return View(procesos);
        }

        // Formulario para agregar proceso a un producto
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            await CargarViewBags();
            return View(new ProcesoFormDTO());
        }

        // Guardar nuevo proceso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(ProcesoFormDTO dto)
        {
            if (!ModelState.IsValid || !dto.Pasos.Any())
            {
                TempData["Error"] = "Debe agregar al menos un paso al proceso.";
                await CargarViewBags();
                return View(dto);
            }

            await _serviceProceso.SaveAsync(dto);
            TempData["Exito"] = "Proceso de preparación guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Formulario para editar proceso
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _serviceProceso.FindFormByIdAsync(id);
            if (dto == null) return NotFound();

            await CargarViewBags();
            return View(dto);
        }

        // Guardar cambios del proceso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ProcesoFormDTO dto)
        {
            if (!ModelState.IsValid || !dto.Pasos.Any())
            {
                TempData["Error"] = "Debe agregar al menos un paso al proceso.";
                await CargarViewBags();
                return View(dto);
            }

            await _serviceProceso.SaveAsync(dto);
            TempData["Exito"] = "Proceso actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Eliminar proceso completo de un producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _serviceProceso.SaveAsync(new ProcesoFormDTO
            {
                IdProducto = id,
                Pasos = new List<PasoFormDTO>()
            });
            TempData["Exito"] = "Proceso eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarViewBags()
        {
            var productos = await _serviceProducto.ListAsync();
            var estaciones = await _serviceProceso.ListEstacionesAsync();

            ViewBag.Productos = new SelectList(
                productos.Where(p => p.Activo),
                "IdProducto", "Nombre");
            ViewBag.Estaciones = estaciones;
        }
    }
}