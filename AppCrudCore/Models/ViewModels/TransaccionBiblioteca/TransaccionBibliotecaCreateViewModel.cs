using AppCrudCore.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppCrudCore.Models.ViewModels.TransaccionBiblioteca
{
    public class TransaccionBibliotecaCreateViewModel
    {
        //[Required]
        //public int ClienteId { get; set; } TABLA VIEJA FK

        //public int? EmpleadoId { get; set; } TATBLA VIEJA FK

        [Required(ErrorMessage = "El monto pagado es obligatorio")]
        public int ClienteUsuarioId { get; set; }

        [Required(ErrorMessage = "El monto pagado es obligatorio")]
        public int EmpleadoUsuarioId { get; set; }



        [Required(ErrorMessage = "Seleccionar el tipo de servicio es obligatorio")]
        public TipoServicio TipoServicio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el origen de la transacción")]
        public OrigenTransaccion Origen { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de servicio para la transacción")]
        public TipoPago TipoPago { get; set; }

        public DateTime? FechaDevolucion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? Observaciones { get; set; }

        public string? ReferenciaPago { get; set; }

        [Required(ErrorMessage = "El monto pagado es obligatorio")]
        public decimal MontoPagado { get; set; }

        [Required(ErrorMessage = "El total es obligatorio")]
        public decimal Total { get; set; }

        public List<CreateTransaccionDetalleViewModel> Detalles { get; set; }
            = new List<CreateTransaccionDetalleViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            //validar monto pagoado mayor que el total
            if (MontoPagado > Total)
            {
                yield return new ValidationResult(
                    "El monto pagado no puede ser mayor que el total de la transacción.",
                    new[] { nameof(MontoPagado) }
                    );
            }

            //validar fecha obligatoria si es prestamo
            if (TipoServicio == TipoServicio.Prestamo)
            {
                if (!FechaDevolucion.HasValue)
                {
                    yield return new ValidationResult(
                        "La fecha de devolución es obligatoria para préstamos.",
                        new[] { nameof(FechaDevolucion) }
                    );
                }
                else if (FechaDevolucion.Value <= FechaCreacion)
                {
                    yield return new ValidationResult(
                        "La fecha de devolución debe ser posterior a la fecha de creación.",
                        new[] { nameof(FechaDevolucion) }
                    );
                }
            }
        }
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
