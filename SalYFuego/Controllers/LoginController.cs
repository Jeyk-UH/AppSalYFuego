using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;

namespace Sal_Fuego.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IServiceUsuario serviceUsuario, ILogger<LoginController> logger)
        {
            _serviceUsuario = serviceUsuario;
            _logger = logger;
        }

        // Formulario de inicio de sesión
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginDTO());
        }

        // Procesar inicio de sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            // Verificar sí el usuario existe y la contraseña es correcta
            var usuarioDTO = await _serviceUsuario.LoginAsync(dto.Correo, dto.Password);
            if (usuarioDTO == null)
            {
                string mensaje = "Correo o contraseña incorrectos, o el usuario está inactivo.";
                ViewBag.Message = mensaje;
                _logger.LogInformation("Error en login de {Correo}, Error --> {Mensaje}", dto.Correo, mensaje);
                return View(dto);
            }

            // Claim almacena información del usuario como nombre, rol y otros
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuarioDTO.IdUsuario.ToString()),
                new(ClaimTypes.Name, usuarioDTO.NombreCompleto),
                new(ClaimTypes.Email, usuarioDTO.Correo),
                new(ClaimTypes.Role, usuarioDTO.NombreRol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties);

            _logger.LogInformation("Conexión correcta de {Correo}", dto.Correo);

            return RedirectToAction("Index", "Home");
        }

        // Formulario de registro de clientes (autoservicio, siempre crea rol Cliente)
        [HttpGet]
        public IActionResult Registro()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegistroDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var error = await _serviceUsuario.RegistrarClienteAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(dto);
            }

            // Iniciar sesión automáticamente tras un registro exitoso
            var usuarioDTO = await _serviceUsuario.LoginAsync(dto.Correo, dto.Password);
            if (usuarioDTO != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, usuarioDTO.IdUsuario.ToString()),
                    new(ClaimTypes.Name, usuarioDTO.NombreCompleto),
                    new(ClaimTypes.Email, usuarioDTO.Correo),
                    new(ClaimTypes.Role, usuarioDTO.NombreRol)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { AllowRefresh = true });
            }

            TempData["Exito"] = "¡Cuenta creada correctamente! Bienvenido a Sal y Fuego.";
            return RedirectToAction("Index", "Home");
        }

        // Cerrar sesión
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Acceso denegado (usuario autenticado sin el rol requerido)
        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
