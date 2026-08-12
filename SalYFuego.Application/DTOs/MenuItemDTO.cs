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
        // Id real del producto o combo (uno de los dos queda null),
        // útil para filtrar catálogos por lo que está en el menú disponible
        public int? IdProducto { get; set; }
        public int? IdCombo { get; set; }
    }
}