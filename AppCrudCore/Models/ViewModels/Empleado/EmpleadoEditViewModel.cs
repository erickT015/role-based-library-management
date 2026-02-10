using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppCrudCore.Models.ViewModels.Empleado
{
    public class EmpleadoEditViewModel
    {

        public int IdEmpleado { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string NombreCompleto { get; set; }


        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20, ErrorMessage = "la cédula no puede superar los 20 caracteres")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string Correo { get; set; }

        // OPCIONAL: solo se valida si viene con valor
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string? NuevaPass { get; set; }

        // Compare solo compara, no hace obligatorio el campo
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string? ConfirmarPass { get; set; }

        [Required(ErrorMessage = "La fecha de contrato es obligatoria")]
        public DateOnly FechaContrato { get; set; }

        public bool Activo { get; set; }

        [Required]
        public int RolId { get; set; }


    }
}

