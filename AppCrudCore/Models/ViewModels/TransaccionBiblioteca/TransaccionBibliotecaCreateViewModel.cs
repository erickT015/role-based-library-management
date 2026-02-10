using AppCrudCore.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppCrudCore.Models.ViewModels.TransaccionBiblioteca
{
    public class TransaccionBibliotecaCreateViewModel
    {
    [Required]
        public int ClienteId { get; set; }

        public int? EmpleadoId { get; set; }

        [Required]
        public TipoServicio TipoServicio { get; set; }

        [Required]
        public OrigenTransaccion Origen { get; set; }

        [Required]
        public TipoPago TipoPago { get; set; }

        public DateTime? FechaDevolucion { get; set; }

        public string? Observaciones { get; set; }

        public string? ReferenciaPago { get; set; }

        public decimal MontoPagado { get; set; }

        public decimal Total { get; set; }

        public List<CreateTransaccionDetalleViewModel> Detalles { get; set; }
            = new List<CreateTransaccionDetalleViewModel>();
    }


    public class CreateTransaccionDetalleViewModel
    {
        [Required]
        public int LibroId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }

}
