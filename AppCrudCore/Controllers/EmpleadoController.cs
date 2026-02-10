using AppCrudCore.Data; //conexion a base de datos
using AppCrudCore.Models; // modelos
using AppCrudCore.Models.ViewModels.Empleado;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace AppCrudCore.Controllers
{


    public class EmpleadoController : Controller
    {

        private readonly AppDBContext _appDBContext;

        public EmpleadoController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        private void CargarRoles()//METODO PRIVADO PARA CARGAR ROLES
        {
            ViewBag.Rol = _appDBContext.Rol
        .Where(r => r.Activo)
        .Select(r => new SelectListItem
        {
            Value = r.IdRol.ToString(),
            Text = r.Nombre
        })
        .ToList();
        }

        //RENDERIZAR LISTA
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _appDBContext.Empleados
                  .Include(empleado => empleado.Rol) //conecta con la tabla de rol
                               .AsNoTracking() // solo lectura
                               .ToListAsync(); //enlistar items

            return View(lista);
        }


        //RENDERIZA FORMULARIO CREAR EMPLEADO
        [HttpGet]
        public IActionResult Nuevo()
        {
            CargarRoles();
            return View();
        }


        //CREAR EMPLEADO
        [HttpPost]
        public async Task<IActionResult> Nuevo(Empleado empleado)
        {
            try
            {
                CargarRoles();

            if (!ModelState.IsValid)
                return View(empleado);

            var Nuevoempleado = new Empleado
            {
                NombreCompleto = empleado.NombreCompleto,
                Correo = empleado.Correo,
                FechaContrato = empleado.FechaContrato,
                Cedula = empleado.Cedula,
                Activo = empleado.Activo,
                RolId = empleado.RolId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(empleado.PasswordHash)
            };


            await _appDBContext.Empleados.AddAsync(Nuevoempleado); //guardar los datos en db
            await _appDBContext.SaveChangesAsync(); //actualizar

            return RedirectToAction(nameof(Lista)); //redirigir a la vista de lista

                //aca va el return
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Ah ocurrido este error"+ ex
                );
                return View(empleado);
            }
        }


        //RENDERIZAR EDITAR EMPLEADO
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            CargarRoles();

            var empleadoDb = await _appDBContext.Empleados
           .FirstOrDefaultAsync(e => e.IdEmpleado == id);

            if (empleadoDb == null)
                return NotFound();

            var empleado = new EmpleadoEditViewModel //usamos el modelView especifico para editar
            {
                IdEmpleado = empleadoDb.IdEmpleado,
                NombreCompleto = empleadoDb.NombreCompleto,
                Correo = empleadoDb.Correo,
                FechaContrato = empleadoDb.FechaContrato,
                Activo = empleadoDb.Activo,
                RolId = empleadoDb.RolId,
                Cedula = empleadoDb.Cedula
            };

            return View(empleado); 
        }

        //EDITAR EMPLEADO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EmpleadoEditViewModel empleado)
        {
            try
            {
                CargarRoles();

                if (!ModelState.IsValid)
                    return View(empleado);

                // Buscar el empleado la base de datos
                var empleadoDb = await _appDBContext.Empleados
                    .FirstOrDefaultAsync(e => e.IdEmpleado == empleado.IdEmpleado);

                // Si no existe o id invalido
                if (empleadoDb == null)
                    return NotFound();

                //  Actualizar los campos con los nuevos datos
                empleadoDb.NombreCompleto = empleado.NombreCompleto;
                empleadoDb.Cedula=empleado.Cedula;
                empleadoDb.Correo = empleado.Correo;
                empleadoDb.FechaContrato = empleado.FechaContrato;
                empleadoDb.Activo = empleado.Activo;
                empleadoDb.RolId = empleado.RolId;

                // Solo entramos aquí si el usuario escribió algo
                if (!string.IsNullOrWhiteSpace(empleado.NuevaPass))
                {
                    //  Validar que ambas contraseñas coincidan
                    if (empleado.NuevaPass != empleado.ConfirmarPass)
                    {
                        // Mostramos en la vista el error
                        ModelState.AddModelError(
                            "ConfirmarPass",
                            "Las contraseñas no coinciden"
                        );

                        // Volvemos a la vista SIN tocar la contraseña anterior
                        return View(empleado);
                    }

                    //  Hashear contraseña
                    empleadoDb.PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(empleado.NuevaPass);
                }

                //  Guardar datos que fueron cambiados
                await _appDBContext.SaveChangesAsync();

                //redirigir a la vista de lista
                return RedirectToAction(nameof(Lista));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "Ocurrió un error al actualizar el empleado"
                );
                return View(empleado);
            }
        }



        //CONSULTAR ELIMINAR USUARIO - NO USADO
        // Muestra la vista de confirmación
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var empleado = await _appDBContext.Empleados
                                              .AsNoTracking() // solo lectura
                                              .FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        //BORRAR EMPLEADO
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var empleado = await _appDBContext.Empleados.FindAsync(id);

                if (empleado == null)
                    return NotFound();

                _appDBContext.Empleados.Remove(empleado);
                await _appDBContext.SaveChangesAsync();

                return RedirectToAction(nameof(Lista));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error al eliminar el empleado.");
                return RedirectToAction(nameof(Lista));
            }
        }
    }
}
