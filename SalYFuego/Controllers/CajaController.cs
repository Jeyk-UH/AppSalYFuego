using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.Config;

namespace Sal_Fuego.Controllers
{
    // Página de inicio del Encargado (cajero): venta a clientes en tienda.
    // Placeholder hasta que se implemente el módulo de Pedidos.
    [Authorize(Roles = Roles.Encargado)]
    public class CajaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
