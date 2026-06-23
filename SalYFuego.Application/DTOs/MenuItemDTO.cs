namespace Sal_Fuego.Aplication.DTOs
{
    public record MenuItemDTO
    {
        // Puede ser producto o combo
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public string CategoriaNombre { get; set; } = null!;
        // Para saber si es producto o combo
        public string Tipo { get; set; } = null!;
    }
}