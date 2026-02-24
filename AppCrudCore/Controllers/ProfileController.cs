using AppCrudCore.Data;
using AppCrudCore.Models.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppCrudCore.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDBContext _context;

        public ProfileController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = await _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == userId);

            if (usuario == null)
                return NotFound();

            var vm = new ProfileIndexViewModel
            {
                Correo = usuario.Correo,
                Cedula = usuario.Cedula,
                NombreCompleto = usuario.NombreCompleto,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion,
                FechaRegistro = usuario.FechaRegistro,
                UltimoLogin = usuario.UltimoLogin,
                RolNombre = usuario.Rol?.Nombre,
                Activo = usuario.Activo
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == userId);

            if (usuario == null)
                return NotFound();

            var vm = new ProfileEditViewModel
            {
                NombreCompleto = usuario.NombreCompleto,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == userId);

            if (usuario == null)
                return NotFound();

            usuario.NombreCompleto = model.NombreCompleto;
            usuario.Telefono = model.Telefono;
            usuario.Direccion = model.Direccion;

            if (!string.IsNullOrWhiteSpace(model.NuevaPassword))
            {
                usuario.PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(model.NuevaPassword);

                usuario.RequiereCambioPassword = false;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Perfil actualizado correctamente";

            return RedirectToAction(nameof(Index));
        }
    }
}
