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

        // Menú disponible según fecha y hora actual. Solo se muestran los renglones
        // cuyo producto/combo sigue activo (uno puede quedar vinculado a un menú
        // viejo aunque ya se haya dado de baja).
        public async Task<IActionResult> Disponible()
        {
            var menu = await _service.GetMenuDisponibleAsync();
            if (menu != null)
                menu.Items = menu.Items.Where(i => i.Activo).ToList();
            return View(menu);
        }
    }
}