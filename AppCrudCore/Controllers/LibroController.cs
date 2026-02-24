using AppCrudCore.Data;
using AppCrudCore.Models;
using AppCrudCore.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCrudCore.Controllers
{

    [Authorize]
    public class LibroController : Controller
    {
        private readonly AppDBContext _context;

        public LibroController(AppDBContext context, ILibroImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        private readonly ILibroImageService _imageService;


        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Index(int? categoriaId, int? anioPublicacion, bool? activo)
        {
            var query = _context.Libro
                .Include(l => l.Categoria)
                .AsQueryable();

            // Filtros existentes...
            if (categoriaId.HasValue) query = query.Where(l => l.CategoriaId == categoriaId);
            if (anioPublicacion.HasValue) query = query.Where(l => l.AnioPublicacion == anioPublicacion);
            if (activo.HasValue) query = query.Where(l => l.Activo == activo);

            // ordenamiento
            query = query.OrderBy(l => l.Titulo);

            var libros = await query.ToListAsync();

            ViewBag.Categorias = new SelectList(await _context.Categoria.ToListAsync(), "IdCategoria", "Nombre", categoriaId);
            ViewBag.AnioPublicacion = anioPublicacion;
            ViewBag.Activo = activo;

            return View(libros);
        }


        // GET: Libro/Details/5
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libro
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(m => m.IdLibro == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libro/Create
        [Authorize(Policy = "AdminOrEmpleado")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "IdCategoria", "Nombre");
            return View();
        }

        // POST: Libro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Create([Bind("IdLibro,Titulo,Autor,ISBN,AnioPublicacion,Resumen,PrecioVenta,StockTotal,StockPrestamo,StockVenta,Activo,CategoriaId")] Libro libro, IFormFile? imagenArchivo)
        {
            if (ModelState.IsValid)
            {
                // guardar imagen usando service
                libro.ImagenUrl =
                    await _imageService
                        .GuardarImagenAsync(imagenArchivo);

                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "IdCategoria", "Nombre", libro.CategoriaId);
            return View(libro);
        }

        // GET: Libro/Edit/5
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libro.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "IdCategoria", "Nombre", libro.CategoriaId);
            return View(libro);
        }

        // POST: Libro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Edit(int id, [Bind("IdLibro,Titulo,Autor,ISBN,AnioPublicacion,Resumen,PrecioVenta,StockTotal,StockPrestamo,StockVenta,Activo,CategoriaId")] Libro libro, IFormFile? imagenArchivo)
        {
            if (id != libro.IdLibro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    // REEMPLAZAR IMAGEN
                    if (imagenArchivo != null)
                    {
                        await _imageService
                            .EliminarImagenAsync(libro.ImagenUrl);

                        libro.ImagenUrl =
                            await _imageService
                                .GuardarImagenAsync(imagenArchivo);
                    }

                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.IdLibro))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "IdCategoria", "Nombre", libro.CategoriaId);
            return View(libro);
        }

        // GET: Libro/Delete/5
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libro
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(m => m.IdLibro == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libro.FindAsync(id);
            if (libro != null)
            {
                libro.Activo = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libro.Any(e => e.IdLibro == id);
        }
    }
}
