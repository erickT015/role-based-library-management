using AppCrudCore.Data;
using AppCrudCore.Models;
using AppCrudCore.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

public abstract class BaseController : Controller
{
    protected bool EsAdmin =>
        User.IsInRole("Admin");

    protected bool EsEmpleado =>
        User.IsInRole("Empleado");

    /// <summary>
    /// Aplica filtro Activo por defecto
    /// </summary>
    protected IQueryable<T> AplicarFiltroActivo<T>(
        IQueryable<T> query,
        bool? activo)
        where T : class, IActivable
    {
        if (!activo.HasValue)
            return query.Where(x => x.Activo);

        if (!EsAdmin)
            return query.Where(x => x.Activo);

        return query.Where(x => x.Activo == activo);
    }

    protected IQueryable<Usuario> AplicarRestriccionUsuario(
    IQueryable<Usuario> query)
    {
        if (EsEmpleado)
        {
            query = query.Where(u => u.RolId == 3); // Cliente
        }

        return query;
    }
}