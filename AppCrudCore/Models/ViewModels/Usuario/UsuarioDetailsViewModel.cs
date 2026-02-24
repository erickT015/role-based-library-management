using System.ComponentModel.DataAnnotations;

namespace AppCrudCore.Models.ViewModels.Usuario
{
    using AppCrudCore.Models;
    public class UsuarioDetailsViewModel
    {

        public string? NombreCompleto { get; set; }
        public string? RolNombre { get; set; }
        public string? Correo { get; set; }
        public string? Cedula { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public bool? Activo { get; set; }
        public int? IdUsuario { get; set; }

        //DATOS PARA ESTADÍSTICAS
        public int? TotalTransacciones { get; set; }
        public int? LibrosPrestados { get; set; }
        public int? LibrosComprados { get; set; }

        public decimal? TotalGastado { get; set; }

        public DateTime? UltimaActividad { get; set; }


    }
}
