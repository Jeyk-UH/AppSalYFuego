using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using X.PagedList.Extensions;

namespace Sal_Fuego.Controllers
{
    // Mantenimiento de usuarios: solo el Administrador puede crear Encargado,
    // Cocina, Repartidor, Salonero u otros Administradores. El Cliente se autoregistra.
    [Authorize(Roles = Roles.Administrador)]
    public class AdminUsuarioController : Controller
    {
        private const int TamanoPagina = 10;

        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceRol _serviceRol;

        public AdminUsuarioController(IServiceUsuario serviceUsuario, IServiceRol serviceRol)
        {
            _serviceUsuario = serviceUsuario;
            _serviceRol = serviceRol;
        }

        // Listado de usuarios, paginado
        public async Task<IActionResult> Index(int? page)
        {
            var usuarios = await _serviceUsuario.ListAsync();
            var paginado = usuarios.ToPagedList(page ?? 1, TamanoPagina);
            return View(paginado);
        }

        // Formulario para agregar
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            await CargarViewBags();
            return View(new UsuarioFormDTO());
        }

        // Guardar nuevo usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(UsuarioFormDTO dto)
        {
            // Al crear, la contraseña es obligatoria
            if (string.IsNullOrWhiteSpace(dto.Password))
                ModelState.AddModelError(nameof(dto.Password), "La contraseña es obligatoria.");

            if (!ModelState.IsValid)
            {
                await CargarViewBags(dto.IdRol);
                return View(dto);
            }

            var error = await _serviceUsuario.AddAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                await CargarViewBags(dto.IdRol);
                return View(dto);
            }

            TempData["Exito"] = "Usuario agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Formulario para editar
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _serviceUsuario.FindFormByIdAsync(id);
            if (dto == null) return NotFound();

            await CargarViewBags(dto.IdRol);
            return View(dto);
        }

        // Guardar cambios del usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(UsuarioFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBags(dto.IdRol);
                return View(dto);
            }

            var error = await _serviceUsuario.UpdateAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                await CargarViewBags(dto.IdRol);
                return View(dto);
            }

            TempData["Exito"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Activar / desactivar usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _serviceUsuario.ToggleActivoAsync(id);
            TempData["Exito"] = "Estado del usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarViewBags(int? idRolSeleccionado = null)
        {
            var roles = await _serviceRol.ListAsync();
            ViewBag.Roles = new SelectList(roles, "IdRol", "NombreRol", idRolSeleccionado);
        }
    }
}
