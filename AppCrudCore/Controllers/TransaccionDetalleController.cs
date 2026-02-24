using AppCrudCore.Data;
using AppCrudCore.Models;
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
    public class TransaccionDetalleController : Controller
    {
        private readonly AppDBContext _context;

        public TransaccionDetalleController(AppDBContext context)
        {
            _context = context;
        }

        // GET: TransaccionDetalle
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Index()
        {
            var query = _context.TransaccionBiblioteca
                .Include(t => t.ClienteUsuario)
                .Include(t => t.EmpleadoUsuario)
                .Include(t => t.Detalles)
                    .ThenInclude(d => d.Libro)
                .AsQueryable();

            //ordenamiento
            query = query.OrderByDescending(t => t.IdTransaccionBiblioteca);

            var transacciones = await query.ToListAsync();

            return View(transacciones);
        }

        // GET: TransaccionDetalle/Details/5
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var transaccion = await _context.TransaccionBiblioteca
                .Include(t => t.ClienteUsuario)
                .Include(t => t.EmpleadoUsuario)
                .Include(t => t.Detalles)
                    .ThenInclude(d => d.Libro)
                .FirstOrDefaultAsync(t =>
                    t.IdTransaccionBiblioteca == id);

            if (transaccion == null)
                return NotFound();

            return View(transaccion);
        }
        private bool TransaccionDetalleExists(int id)
        {
            return _context.TransaccionDetalle.Any(e => e.IdTransaccionDetalle == id);
        }
    }
}
