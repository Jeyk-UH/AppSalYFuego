using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;

namespace Sal_Fuego.Controllers
{
    // Página de inicio de Cocina: pedidos en preparación en tiempo real.
    // Placeholder hasta que se implemente el módulo de Pedidos.
    [Authorize(Roles = Roles.Cocina)]
    public class CocinaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
