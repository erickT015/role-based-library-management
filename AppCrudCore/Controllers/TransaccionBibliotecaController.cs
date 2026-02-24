using AppCrudCore.Data;
using AppCrudCore.Models;
using AppCrudCore.Models.Enums;
using AppCrudCore.Models.ViewModels.TransaccionBiblioteca;
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
    public class TransaccionBibliotecaController : Controller
    {
        private readonly AppDBContext _context;

        public TransaccionBibliotecaController(AppDBContext context)
        {
            _context = context;
        }

        private async Task CargarViewBag()
        {
            ViewBag.Clientes = new SelectList(
                await _context.Usuario
                    .Where(u => u.Activo && u.Rol.Nombre == "Cliente")
                    .ToListAsync(),
                "IdUsuario",
                "NombreCompleto"
            );

            ViewBag.Empleados = new SelectList(
                await _context.Usuario
                    .Where(u => u.Activo &&
                        (u.Rol.Nombre == "Empleado" || u.Rol.Nombre == "Admin"))
                    .ToListAsync(),
                "IdUsuario",
                "NombreCompleto"
            );
        }

        private async Task CargarUsuariosSegunRol()
        {
            int usuarioLogueado =
                int.Parse(User.FindFirstValue(
                    ClaimTypes.NameIdentifier));

            // CLIENTE
            if (User.IsInRole("Cliente"))
            {
                // cliente = él mismo
                ViewBag.Clientes = new SelectList(
                    await _context.Usuario
                        .Where(u => u.IdUsuario == usuarioLogueado)
                        .Select(u => new
                        {
                            u.IdUsuario,
                            u.NombreCompleto
                        })
                        .ToListAsync(),
                    "IdUsuario",
                    "NombreCompleto",
                    usuarioLogueado
                );

                // empleado sistema
                const int EMPLEADO_WEB_ID = 5;

                ViewBag.Empleados = new SelectList(
                    await _context.Usuario
                        .Where(u => u.IdUsuario == EMPLEADO_WEB_ID)
                        .Select(u => new
                        {
                            u.IdUsuario,
                            u.NombreCompleto
                        })
                        .ToListAsync(),
                    "IdUsuario",
                    "NombreCompleto",
                    EMPLEADO_WEB_ID
                );
            }
            // ===============================
            // EMPLEADO / ADMIN
            // ===============================
            else
            {
                // todos los clientes
                ViewBag.Clientes = new SelectList(
                    await _context.Usuario
                        .Where(u => u.Rol.Nombre == "Cliente")
                        .Select(u => new
                        {
                            u.IdUsuario,
                            u.NombreCompleto
                        })
                        .ToListAsync(),
                    "IdUsuario",
                    "NombreCompleto"
                );

                // empleado = logueado
                ViewBag.Empleados = new SelectList(
                    await _context.Usuario
                        .Where(u => u.IdUsuario == usuarioLogueado)
                        .Select(u => new
                        {
                            u.IdUsuario,
                            u.NombreCompleto
                        })
                        .ToListAsync(),
                    "IdUsuario",
                    "NombreCompleto",
                    usuarioLogueado
                );
            }
        }


        // GET: TransaccionBiblioteca
        [Authorize]
        public async Task<IActionResult> Index(
    int? servicioId,
    int? estadoId,
    int? origenId,
    DateOnly? fecha)
        {
            var userIdString =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            int userId = int.Parse(userIdString);

            IQueryable<TransaccionBiblioteca> query =
                _context.TransaccionBiblioteca
                .Include(t => t.ClienteUsuario)
                .Include(t => t.EmpleadoUsuario)
                .Include(t => t.Usuario)
                .Include(t => t.Detalles)
                    .ThenInclude(d => d.Libro)
                .AsQueryable();

            // =========================
            // CLIENTE SOLO SUS DATOS
            // =========================
            if (User.IsInRole("Cliente"))
                query = query.Where(t =>
                    t.ClienteUsuarioId == userId);

            // =========================
            // ENUM FILTROS
            // =========================

            if (servicioId.HasValue &&
                Enum.IsDefined(typeof(TipoServicio), servicioId))
            {
                var tipo = (TipoServicio)servicioId.Value;
                query = query.Where(t =>
                    t.TipoServicio == tipo);
            }

            if (estadoId.HasValue &&
                Enum.IsDefined(typeof(EstadoTransaccion), estadoId))
            {
                var estado =
                    (EstadoTransaccion)estadoId.Value;

                query = query.Where(t =>
                    t.Estado == estado);
            }

            if (origenId.HasValue &&
                Enum.IsDefined(typeof(OrigenTransaccion), origenId))
            {
                var origen =
                    (OrigenTransaccion)origenId.Value;

                query = query.Where(t =>
                    t.Origen == origen);
            }

            // =========================
            // FECHA
            // =========================
            if (fecha.HasValue)
            {
                query = query.Where(t =>
                    DateOnly.FromDateTime(
                        t.FechaCreacion) == fecha.Value);
            }

            // =========================
            // DROPDOWNS ENUM
            // =========================

            ViewBag.Servicios =
                CrearEnumSelectList<TipoServicio>(servicioId);

            ViewBag.Estados =
                CrearEnumSelectList<EstadoTransaccion>(estadoId);

            ViewBag.Origenes =
                CrearEnumSelectList<OrigenTransaccion>(origenId);

            ViewBag.Fecha = fecha;

            return View(await query.ToListAsync());
        }


        //metodo generico para crear select list de enums
        private SelectList CrearEnumSelectList<T>(int? selected)
    where T : Enum
        {
            var values = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new
                {
                    Id = Convert.ToInt32(e),
                    Nombre = e.ToString()
                });

            return new SelectList(values,
                "Id",
                "Nombre",
                selected);
        }


        // GET: TransaccionBiblioteca/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaccionBiblioteca = await _context.TransaccionBiblioteca
                .Include(t => t.Usuario)
                .Include(t => t.ClienteUsuario)
                .Include(t => t.EmpleadoUsuario)
                .Include(t => t.Detalles)
                    .ThenInclude(l => l.Libro)
                .FirstOrDefaultAsync(m => m.IdTransaccionBiblioteca == id);
            if (transaccionBiblioteca == null)
            {
                return NotFound();
            }

            return View(transaccionBiblioteca);
        }


        // GET: Funcion de busqueda por debounce
        [HttpGet]
        public async Task<IActionResult> BuscarLibros(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var libros = await _context.Libro
                .Where(libro => libro.Activo &&
                       (libro.Titulo.Contains(term) || libro.ISBN.Contains(term)))
                .OrderBy(libro => libro.Titulo)
                .Take(20)
                .Select(l => new
                {
                    id = l.IdLibro,
                    titulo = l.Titulo,
                    isbn = l.ISBN,
                    precio = l.PrecioVenta,
                    stockVenta = l.StockVenta,
                    precioPrestamo = l.PrecioPrestamo,
                    stockPrestamo = l.StockPrestamo,
                    stockTotal = l.StockTotal,
                    text = $"{l.Titulo} | ISBN: {l.ISBN} | Sale: {l.StockVenta} | Rent: {l.StockPrestamo}"
                })
                .ToListAsync();

            return Json(libros);
        }


        //GET
        [Authorize]
        public async Task<IActionResult> Create()
        {
            await CargarUsuariosSegunRol();
            return View();
        }


        //funcion para obtener el numero de transaccion
        private async Task<string> GenerarNumeroTransaccion()
        {
            var fecha = DateTime.Now;

            var ultimoId = await _context.TransaccionBiblioteca
                .OrderByDescending(t => t.IdTransaccionBiblioteca)
                .Select(t => t.IdTransaccionBiblioteca)
                .FirstOrDefaultAsync();

            var siguiente = ultimoId + 1;

            return $"TX-{fecha:yyyyMMdd}-{siguiente:D6}";
        }


        //POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(
            TransaccionBibliotecaCreateViewModel model)
        {
            await CargarUsuariosSegunRol();

            if (!ModelState.IsValid)
            return View(model);     

            // VALIDAR DETALLES
            if (model.Detalles == null || !model.Detalles.Any())
            {
                ModelState.AddModelError("",
                    "Debe agregar al menos un libro.");
                return View(model);
            }

            // USUARIO LOGUEADO
            int usuarioLogueado = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // CLIENTE NO PUEDE PRESTAR
            if (User.IsInRole("Cliente") &&
                model.TipoServicio == TipoServicio.Prestamo)
            {
                ModelState.AddModelError("",
                    "Los clientes no pueden realizar préstamos.");
                return View(model);
            }

            using var dbTransaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var transaccion = new TransaccionBiblioteca
                {
                    NumeroTransaccion =
                        await GenerarNumeroTransaccion(),

                    TipoServicio = model.TipoServicio,
                    Origen = model.Origen,
                    TipoPago = model.TipoPago,

                    FechaCreacion = DateTime.Now,

                    FechaDevolucion =
                        model.TipoServicio == TipoServicio.Prestamo
                        ? model.FechaDevolucion
                        : null,

                    ReferenciaPago = model.ReferenciaPago,
                    Observaciones = model.Observaciones,

                    Estado =
                        model.TipoServicio == TipoServicio.Prestamo
                        ? EstadoTransaccion.Prestado
                        : EstadoTransaccion.Vendido
                };

                //  ASIGNACIÓN SEGURA USUARIOS

                if (User.IsInRole("Cliente"))
                {
                    // Cliente compra para sí mismo
                    transaccion.ClienteUsuarioId =
                        usuarioLogueado;

                    // empleado del sistema
                    transaccion.EmpleadoUsuarioId =
                        model.EmpleadoUsuarioId;

                    transaccion.UsuarioId =
                        usuarioLogueado;
                }
                else
                {
                    // empleado autenticado atiende
                    transaccion.EmpleadoUsuarioId =
                        usuarioLogueado;

                    transaccion.ClienteUsuarioId =
                        model.ClienteUsuarioId;

                    transaccion.UsuarioId =
                        usuarioLogueado;
                }

                decimal total = 0;
                // DETALLES MULTILIBRO
                foreach (var item in model.Detalles)
                {
                    var libro = await _context.Libro
                        .FirstOrDefaultAsync(l =>
                            l.IdLibro == item.LibroId);

                    if (libro == null)
                        throw new Exception("Libro no existe");

                    decimal precioUnitario =
                        model.TipoServicio == TipoServicio.Prestamo
                        ? libro.PrecioPrestamo
                        : libro.PrecioVenta;

                    // VALIDAR STOCK
                    if (model.TipoServicio == TipoServicio.Venta)
                    {
                        if (libro.StockVenta < item.Cantidad)
                            throw new Exception(
                                $"Stock insuficiente: {libro.Titulo}");

                        libro.StockVenta -= item.Cantidad;
                        libro.StockTotal -= item.Cantidad;
                    }
                    else
                    {
                        if (libro.StockPrestamo < item.Cantidad)
                            throw new Exception(
                                $"Stock préstamo insuficiente: {libro.Titulo}");

                        libro.StockPrestamo -= item.Cantidad;
                    }

                    var subtotal =
                        precioUnitario * item.Cantidad;

                    transaccion.Detalles.Add(
                        new TransaccionDetalle
                        {
                            LibroId = item.LibroId,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = precioUnitario,
                            Subtotal = subtotal
                        });

                    total += subtotal;
                }

                // TOTALES
                transaccion.Total = total;
                transaccion.MontoPagado = model.MontoPagado;
                if (transaccion.MontoPagado < transaccion.Total)
                    throw new Exception("Pago insuficiente.");

                _context.TransaccionBiblioteca.Add(transaccion);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();

                ModelState.AddModelError("",
                    ex.InnerException?.Message ?? ex.Message);

                await CargarUsuariosSegunRol();
                return View(model);
            }
        }



        //GET: EDIT
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var transaccion = await _context.TransaccionBiblioteca
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdTransaccionBiblioteca == id);

            if (transaccion == null)
                return NotFound();

            var model = new TransaccionBibliotecaEditViewModel
            {
                IdTransaccionBiblioteca = transaccion.IdTransaccionBiblioteca,
                Estado = transaccion.Estado,
                FechaDevolucion = transaccion.FechaDevolucion,
                Observaciones = transaccion.Observaciones
            };

            return View(model);
        }


        //POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrEmpleado")]
        public async Task<IActionResult> Edit(TransaccionBibliotecaEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var transaccion = await _context.TransaccionBiblioteca
                .FirstOrDefaultAsync(x => x.IdTransaccionBiblioteca == model.IdTransaccionBiblioteca);

            if (transaccion == null)
                return NotFound();

            try
            {
                if (transaccion.Estado == EstadoTransaccion.Cancelado)
                {
                    ModelState.AddModelError("", "No se puede editar una transacción cancelada.");
                    return View(model);
                }

                transaccion.Estado = model.Estado;
                transaccion.FechaDevolucion = model.FechaDevolucion;
                transaccion.Observaciones = model.Observaciones;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        private bool TransaccionBibliotecaExists(int id)
        {
            return _context.TransaccionBiblioteca.Any(e => e.IdTransaccionBiblioteca == id);
        }
    }
}
