using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;

namespace Sal_Fuego.Controllers
{
    // Página de inicio del Repartidor: pedidos de delivery asignados.
    // Placeholder hasta que se implemente el módulo de Pedidos.
    [Authorize(Roles = Roles.Repartidor)]
    public class RepartidorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
