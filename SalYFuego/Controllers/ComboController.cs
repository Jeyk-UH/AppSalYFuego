using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    public class ComboController : Controller
    {
        private readonly IServiceCombo _service;

        public ComboController(IServiceCombo service) => _service = service;

        public async Task<IActionResult> Index()
        {
            var combos = await _service.ListAsync();
            return View(combos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var combo = await _service.FindByIdAsync(id);
            return combo == null ? NotFound() : View(combo);
        }
    }
}