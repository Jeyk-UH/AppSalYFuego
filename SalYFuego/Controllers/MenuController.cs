using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    public class MenuController : Controller
    {
        private readonly IServiceMenu _service;

        public MenuController(IServiceMenu service) => _service = service;

        // Listado de todos los menús
        public async Task<IActionResult> Index()
        {
            var menus = await _service.ListAsync();
            return View(menus);
        }

        // Menú disponible según fecha y hora actual
        public async Task<IActionResult> Disponible()
        {
            var menu = await _service.GetMenuDisponibleAsync();
            return View(menu);
        }
    }
}