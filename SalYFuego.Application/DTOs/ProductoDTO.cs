namespace Sal_Fuego.Aplication.DTOs
{
    public record ProductoDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
        // Nombre de la categoría
        public string CategoriaNombre { get; set; } = null!;
        // Lista de ingredientes
        public List<IngredienteDTO> Ingredientes { get; set; } = new();
        // Imagen principal
        public string? ImagenUrl { get; set; }
    }
}