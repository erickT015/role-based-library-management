using AppCrudCore.Data;
using AppCrudCore.Models;
using AppCrudCore.Models.Enums;
using AppCrudCore.Models.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppCrudCore.Controllers
{
    [Authorize]
    public class UsuarioController : BaseController
    {
        private readonly AppDBContext _context;

        public UsuarioController(AppDBContext context)
        {
            _context = context;
        }

        private async Task CargarRolesPorUsuario()
        {
            IQueryable<Rol> query = _context.Rol
                .Where(r => r.Activo);


            if (User.IsInRole("Empleado"))            // EMPLEADO → SOLO CLIENTE
            {
                query = query.Where(r => r.Nombre == "Cliente");
            }

            if (User.IsInRole("Cliente"))            // CLIENTE → NO CAMBIA ROLES
            {
                ViewBag.RolId = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.BloquearRol = true;
                return;
            }

            var roles = await query
                .Select(r => new SelectListItem
                {
                    Value = r.IdRol.ToString(),
                    Text = r.Nombre
                })
                .ToListAsync();

            ViewBag.RolId = roles;
        }

        // GET: Usuario
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Index(int? rolId, bool? activo)
        {
            var query = _context.Usuario
                .Include(u => u.Rol)
                .AsNoTracking()
                .AsQueryable();

            //filtros existentes
            query = AplicarFiltroActivo(query, activo); // filtro por query desde base controller
            query = AplicarRestriccionUsuario(query); // Restricción empleado query desde base controller
            if (rolId.HasValue)  query = query.Where(u => u.RolId == rolId);

            //ordenamiento
            query = query.OrderBy(u => u.Cedula);

            var usuarios = await query.ToListAsync();

            ViewBag.Roles = new SelectList( await _context.Rol.ToListAsync(), "IdRol", "Nombre", rolId );

            ViewBag.RolId = rolId;
            ViewBag.Activo = activo ?? true;

            return View(usuarios);
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var query = _context.Usuario
                .Include(u => u.Rol)
                .AsNoTracking()
                .AsQueryable();

            // Restricción empleado query desde base controller
            query = AplicarRestriccionUsuario(query);

            var usuario = await query.FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            // TRANSACCIONES DEL USUARIO
            var transacciones = await _context.TransaccionBiblioteca
                .Where(t => t.ClienteUsuarioId == id)
                .Include(t => t.Detalles)
                .ToListAsync();

            // VIEWMODEL
            var vm = new UsuarioDetailsViewModel
            {
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                Cedula = usuario.Cedula,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion,
                RolNombre = usuario.Rol?.Nombre,
                FechaRegistro = usuario.FechaRegistro,
                UltimoLogin = usuario.UltimoLogin,
                Activo = usuario.Activo,

                // ===== Estadísticas =====
                TotalTransacciones = transacciones.Count,

                LibrosPrestados =
                    transacciones
                        .SelectMany(t => t.Detalles)
                        .Sum(d => d.Cantidad),

                TotalGastado =
                    transacciones.Sum(t => t.Total),

                UltimaActividad =
                    transacciones
                        .OrderByDescending(t => t.FechaCreacion)
                        .Select(t => (DateTime?)t.FechaCreacion)
                        .FirstOrDefault()
            };

            return View(vm);
        }

        // GET: Usuario/Create
        public async Task<IActionResult> Create()
        {
            await CargarRolesPorUsuario();
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Create(UsuarioCreateViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarRolesPorUsuario(); // Recargar los roles para la vista
                    return View(vm);
                }

                // Crear la entidad Usuario
                var nuevoUsuario = new Usuario
                {
                    Correo = vm.Correo,
                    Cedula = vm.Cedula,
                    NombreCompleto = vm.NombreCompleto,
                    Telefono = vm.Telefono,
                    Direccion = vm.Direccion,
                    RolId = vm.RolId,
                    Activo = true,
                    FechaRegistro = DateTime.Now,
                    UltimoLogin = null,
                    RequiereCambioPassword = false,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password)
                };

                // Asignarle rol despues de crear objeto
                if (User.IsInRole("Empleado"))
                {
                    nuevoUsuario.RolId = 3; // Cliente en DB
                }

                // Guardar en DB
                await _context.Usuario.AddAsync(nuevoUsuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await CargarRolesPorUsuario();
                ModelState.AddModelError("", "Ha ocurrido un error: " + ex.Message);
                return View(vm);
            }
        }


        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var query = _context.Usuario
        .Include(u => u.Rol)
        .AsQueryable();

            // Restricción empleado query desde base controller
            query = AplicarRestriccionUsuario(query);

            var usuarioDb = await query
        .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuarioDb == null)
                return NotFound();

            var vm = new UsuarioEditViewModel
            {
                IdUsuario = usuarioDb.IdUsuario,
                Correo = usuarioDb.Correo,
                Cedula = usuarioDb.Cedula,
                NombreCompleto = usuarioDb.NombreCompleto,
                Telefono = usuarioDb.Telefono,
                Direccion = usuarioDb.Direccion,
                Activo = usuarioDb.Activo,
                RolId = usuarioDb.RolId
            };

            await CargarRolesPorUsuario();

            return View(vm);
        }

        // POST: Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioEditViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarRolesPorUsuario();
                    return View(vm);
                }

                var query = _context.Usuario.AsQueryable();

                // Restricción empleado query desde base controller
                query = AplicarRestriccionUsuario(query);

                var usuarioDb = await query
        .FirstOrDefaultAsync(u => u.IdUsuario == vm.IdUsuario);

                if (usuarioDb == null)
                    return NotFound();

                // actualizar campos editables
                usuarioDb.Correo = vm.Correo;
                usuarioDb.Cedula = vm.Cedula;
                usuarioDb.NombreCompleto = vm.NombreCompleto;
                usuarioDb.Telefono = vm.Telefono;
                usuarioDb.Direccion = vm.Direccion;
                usuarioDb.Activo = vm.Activo;
                if (User.IsInRole("Admin"))
                {
                    usuarioDb.RolId = vm.RolId;
                }

                // actualizar password solo si se ingresó una nueva
                if (!string.IsNullOrWhiteSpace(vm.NuevaPassword))
                {
                    if (vm.NuevaPassword.Length < 8)
                    {
                        ModelState.AddModelError("NuevaPassword", "Debe tener al menos 8 caracteres");
                        await CargarRolesPorUsuario();
                        return View(vm);
                    }

                    if (vm.NuevaPassword != vm.ConfirmarPassword)
                    {
                        ModelState.AddModelError("ConfirmarPassword", "Las contraseñas no coinciden");
                        await CargarRolesPorUsuario();
                        return View(vm);
                    }

                    usuarioDb.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.NuevaPassword);

                    usuarioDb.RequiereCambioPassword = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Ah ocurrido este error" + ex
                );
                await CargarRolesPorUsuario();
                return View(vm);
            }
        }

        // GET: Usuario/Delete/5
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioDb = await _context.Usuario
                .Include(u => u.Rol)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuarioDb == null)
                return NotFound();

            var adminId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // evitar auto eliminación
            if (usuarioDb.IdUsuario == adminId)
                return BadRequest();

            var vm = new UsuarioDeleteViewModel
            {
                IdUsuario = usuarioDb.IdUsuario,
                Correo = usuarioDb.Correo,
                Cedula = usuarioDb.Cedula,
                NombreCompleto = usuarioDb.NombreCompleto,
                Telefono = usuarioDb.Telefono,
                Direccion = usuarioDb.Direccion,
                Activo = usuarioDb.Activo,
                NombreRol = usuarioDb.Rol?.Nombre
            };

            return View(vm);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioDb = await _context.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuarioDb == null)
                return NotFound();

            var adminId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // evitar auto eliminación
            if (usuarioDb.IdUsuario == adminId)
                return BadRequest();

            // SOFT DELETE
            usuarioDb.Activo = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.IdUsuario == id);
        }
    }
}
