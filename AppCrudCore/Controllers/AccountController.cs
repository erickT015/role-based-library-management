using AppCrudCore.Data;
using AppCrudCore.Models;
using AppCrudCore.Models.ViewModels.Account;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppCrudCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _context;

        public AccountController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == model.Correo);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos");
                return View(model);
            }

            bool passwordValido = BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash);

            if (!passwordValido)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos");
                return View(model);
            }

            usuario.UltimoLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
        new Claim(ClaimTypes.Name, usuario.NombreCompleto),
        new Claim(ClaimTypes.Email, usuario.Correo),
        new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "SinRol"),
        new Claim("Cedula", usuario.Cedula),
        new Claim("RequiereCambioPassword", usuario.RequiereCambioPassword.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(
        claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            /*if (usuario.RequiereCambioPassword)
            {
                return RedirectToAction("CambiarPassword");
            }*/

            return RedirectToAction("Index", "Home");
        }

        // POST: ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Login");

            // lógica futura
            return RedirectToAction("Login");
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // verificar correo existente
            bool existeCorreo = await _context.Usuario
                .AnyAsync(u => u.Correo == model.Correo);

            if (existeCorreo)
            {
                ModelState.AddModelError("", "El correo ya existe. sugerimos presionar recuperar contraseña");
                return View(model);
            }

            bool existeCedula = await _context.Usuario
               .AnyAsync(u => u.Cedula == model.Cedula);

            if (existeCedula)
            {
                ModelState.AddModelError("", "Cedula existente en el sistema, pongase en contacto con el Admin.");
                return View(model);
            }

            var usuario = new Usuario
            {
                Correo = model.Correo,
                Cedula = model.Cedula,
                NombreCompleto = model.NombreCompleto,
                Telefono = model.Telefono,
                Direccion = model.Direccion,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(model.Password),

                FechaRegistro = DateTime.Now,
                Activo = true,

                RolId = 3 // Cliente
            };

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        //POST: LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

    }
}
