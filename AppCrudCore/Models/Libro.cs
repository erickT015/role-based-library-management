using System.ComponentModel.DataAnnotations;

namespace AppCrudCore.Models
{
    public class Libro : IValidatableObject
    {

        public int IdLibro { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(150, ErrorMessage = "El autor no puede superar los 150 caracteres")]
        public string Autor { get; set; }


        [Required(ErrorMessage = "El ISBN es obligatorio")]
        public string ISBN {  get; set; }


        [Required(ErrorMessage = "El año de publicación es obligatorio")]
        public int AnioPublicacion { get; set; }


        [Required(ErrorMessage = "El resumen es obligatorio")]
        [StringLength(500, ErrorMessage = "El resumen no puede superar los 500 caracteres")]
        public string Resumen {  get; set; }


        [Required(ErrorMessage = "El precio de venta es obligatorio")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
        public decimal PrecioVenta { get; set; }


        [Required(ErrorMessage = "La cantidad total es obligatoria")]
        [Range(0,int.MaxValue)] //minimo 0, no negativos]
        public int StockTotal { get; set; }


        [Range(0, int.MaxValue)]
        public int StockPrestamo { get; set; }


        [Range(0, int.MaxValue)]
        public int StockVenta { get; set; }


        [Required]
        public bool Activo {  get; set; }


        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; } //FK
        public Categoria? Categoria { get; set; }   // navegación (lectura)

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StockPrestamo + StockVenta > StockTotal)
            {
                yield return new ValidationResult(
                    "StockPrestamo + StockVenta no puede ser mayor que StockTotal.",
                    new[] { nameof(StockPrestamo), nameof(StockVenta), nameof(StockTotal) }
                );
            }

            if (AnioPublicacion > DateTime.Now.Year)
            {
                yield return new ValidationResult(
                    $"El año no puede ser mayor que {DateTime.Now.Year}.",
                    new[] { nameof(AnioPublicacion) }
                );
            }
        }


    }
}
