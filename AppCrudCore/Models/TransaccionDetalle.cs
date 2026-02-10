using System.ComponentModel.DataAnnotations;

namespace AppCrudCore.Models
{
    public class TransaccionDetalle : IValidatableObject
    {
        public int IdTransaccionDetalle { get; set; }


        [Required]
        public int TransaccionBibliotecaId { get; set; } //FK
        public TransaccionBiblioteca? TransaccionBiblioteca { get; set; }


        [Required]
        public int LibroId { get; set; } //FK
        public Libro? Libro { get; set; }


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }


        [Required, Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que 0.")]
        public decimal PrecioUnitario { get; set; }


        [Required, Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que 0.")]
        public decimal Subtotal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Subtotal != Cantidad * PrecioUnitario)
            {
                yield return new ValidationResult(
                    "Subtotal debe ser igual a Cantidad × PrecioUnitario.",
                    new[] { nameof(Subtotal) }
                );
            }
        }
    }
}
