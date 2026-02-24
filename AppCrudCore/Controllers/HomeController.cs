using AppCrudCore.Data;
using AppCrudCore.Models;
using AppCrudCore.Models.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AppCrudCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext _context;

        public HomeController(AppDBContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _context.Libro
                .Include(l => l.Categoria)
                .Where(l => l.Activo)
                .AsNoTracking()
                .GroupBy(l => l.Categoria!.Nombre)
                .Select(g => new HomeCategoriaVM
                {
                    CategoriaNombre = g.Key,
                    Libros = g.ToList()
                })
                .ToListAsync();

            return View(data);
        }


        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
