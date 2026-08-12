using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;

namespace Sal_Fuego.Controllers
{
    // Página de inicio del Salonero: entrega de pedidos listos en sala
    // y empaque de los pedidos de delivery. Placeholder hasta el módulo de Pedidos.
    [Authorize(Roles = Roles.Salonero)]
    public class SaloneroController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
