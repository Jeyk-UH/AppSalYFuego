using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    public class ProcesoController : Controller
    {
        private readonly IServiceProceso _service;

        public ProcesoController(IServiceProceso service) => _service = service;

        // Listado de todos los procesos de preparación
        public async Task<IActionResult> Index()
        {
            var procesos = await _service.ListAsync();
            return View(procesos);
        }

        // Detalle del proceso de preparación de un producto
        public async Task<IActionResult> Detalle(int id)
        {
            var proceso = await _service.FindByIdAsync(id);
            return proceso == null ? NotFound() : View(proceso);
        }
    }
}