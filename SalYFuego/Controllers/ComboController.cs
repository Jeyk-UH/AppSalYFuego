using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sal_Fuego.Controllers
{
    public class ComboController : Controller
    {
        private const int TamanoPagina = 12;

        private readonly IServiceCombo _service;
        private readonly IServiceMenu _serviceMenu;

        public ComboController(IServiceCombo service, IServiceMenu serviceMenu)
        {
            _service = service;
            _serviceMenu = serviceMenu;
        }

        // Solo los combos activos y que estén dentro del menú disponible ahora
        public async Task<IActionResult> Index(int? page)
        {
            var menuDisponible = await _serviceMenu.GetMenuDisponibleAsync();
            if (menuDisponible == null)
                return View(Enumerable.Empty<Sal_Fuego.Aplication.DTOs.ComboDTO>()
                    .ToPagedList(page ?? 1, TamanoPagina));

            var idsDisponibles = menuDisponible.Items
                .Where(i => i.Tipo == "Combo" && i.IdCombo.HasValue)
                .Select(i => i.IdCombo!.Value)
                .ToHashSet();

            var combos = await _service.ListAsync();
            var disponibles = combos
                .Where(c => c.Activo && idsDisponibles.Contains(c.IdCombo))
                .ToList();

            return View(disponibles.ToPagedList(page ?? 1, TamanoPagina));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var combo = await _service.FindByIdAsync(id);
            return combo == null ? NotFound() : View(combo);
        }
    }
}