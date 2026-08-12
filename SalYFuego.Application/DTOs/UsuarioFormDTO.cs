using System.ComponentModel.DataAnnotations;

namespace Sal_Fuego.Aplication.DTOs
{
    // DTO de formulario para el mantenimiento de usuarios (solo Administrador)
    public record UsuarioFormDTO
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 200 caracteres")]
        public string? NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        public string? Correo { get; set; }

        // Solo obligatoria al crear; en edición se deja vacía si no se desea cambiar
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int IdRol { get; set; }

        public bool Activo { get; set; } = true;
    }
}
