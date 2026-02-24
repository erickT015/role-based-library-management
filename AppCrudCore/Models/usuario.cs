﻿using System.ComponentModel.DataAnnotations;
using AppCrudCore.Models.Interfaces;

namespace AppCrudCore.Models
{
    public class Usuario : IActivable
    {
        public int IdUsuario { get; set; }

        //IDENTIDAD
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres")]
        public string Correo { get; set; }


        [Required]
        public String PasswordHash { get; set; }

        //CONTACTO
        [Required(ErrorMessage = "la cédula es obligatorio")]
        [StringLength(20, ErrorMessage = "La cédula no puede superar los 20 caracteres")]
        public string Cedula { get; set; }


        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
        public string NombreCompleto { get; set; }


        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres")]
        public string Telefono { get; set; }


        [StringLength(250, ErrorMessage = "la dirección no puede superar los 250 caracteres")]
        public string Direccion {  get; set; }

        //SEGURIDAD
        [Required(ErrorMessage = "La fecha de registro es obligatorio")]
        public DateTime FechaRegistro {  get; set; } = DateTime.Now;

        public DateTime? UltimoLogin { get; set; }

        public bool RequiereCambioPassword { get; set; } = false;


        [Required]
        public bool Activo { get; set; }

        public int? RolId { get; set; } //FK Rol
        public Rol? Rol { get; set; }     // navegación (lectura)
    }
}