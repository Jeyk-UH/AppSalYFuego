namespace Sal_Fuego.Aplication.DTOs
{
    // Resultado de la búsqueda de clientes registrados desde Caja
    public record ClienteBusquedaDTO
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Correo { get; set; }
        public string? Cedula { get; set; }
    }
}
