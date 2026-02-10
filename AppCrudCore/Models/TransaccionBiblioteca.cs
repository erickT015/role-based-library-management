using AppCrudCore.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AppCrudCore.Models
{
    public class TransaccionBiblioteca : IValidatableObject
    {
        public int IdTransaccionBiblioteca { get; set; }


        [Required, StringLength(50)]
        public string NumeroTransaccion { get; set; }

        [Required]
        public int ClienteId { get; set; } //FK
        public Cliente Cliente { get; set; }

        public int? EmpleadoId { get; set; } //FK
        public Empleado? Empleado { get; set; }


        [Required]
        public TipoServicio TipoServicio { get; set; }


        [Required]
        public OrigenTransaccion Origen { get; set; }


        [Required]
        public EstadoTransaccion Estado { get; set; }


        [Required]
        public TipoPago TipoPago { get; set; }


        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Total debe ser mayor que 0.")]
        public decimal Total { get; set; }


        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Monto pagado debe ser mayor que 0.")]
        public decimal MontoPagado { get; set; }


        public string? ReferenciaPago { get; set; }


        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;


        public DateTime? FechaDevolucion { get; set; }

        public DateTime? FechaCompletada { get; set; }


        public string? Observaciones { get; set; }

        public ICollection<TransaccionDetalle> Detalles { get; set; } = new List<TransaccionDetalle>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //validar suma de detalles
            if (Detalles != null && Detalles.Any())
            {
                var sumaDetalles = Detalles.Sum(d => d.Subtotal);

                if (Total != sumaDetalles)
                {
                    yield return new ValidationResult(
                        "Total debe coincidir con la suma de los Subtotales.",
                        new[] { nameof(Total) }
                    );
                }
            }

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
}
