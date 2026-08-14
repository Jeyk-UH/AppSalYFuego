namespace Sal_Fuego.Aplication.DTOs
{
    // DTO de listado / datos de sesión del usuario autenticado
    public record UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public bool Activo { get; set; }
        public int IdRol { get; set; }
        // Nombre del rol (para Claims y para mostrar en pantalla)
        public string NombreRol { get; set; } = null!;
        // Opcional: usada para buscar clientes registrados desde Caja
        public string? Cedula { get; set; }
    }
}
