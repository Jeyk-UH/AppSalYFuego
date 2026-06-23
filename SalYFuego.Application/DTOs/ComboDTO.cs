namespace Sal_Fuego.Aplication.DTOs
{
    public record ComboDTO
    {
        public int IdCombo { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PrecioEspecial { get; set; }
        public bool Activo { get; set; }
        // Nombre de la categoría
        public string CategoriaNombre { get; set; } = null!;
        // Imagen del combo
        public string? UrlImagen { get; set; }
        // Lista de productos que componen el combo
        public List<ComboProductoDTO> Productos { get; set; } = new();
    }
}