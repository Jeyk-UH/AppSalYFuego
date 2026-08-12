namespace Sal_Fuego.Aplication.DTOs
{
    public record ComboDTO
    {
  
        public int IdCombo { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PrecioEspecial { get; set; }
        public bool Activo { get; set; }
        public int IdCategoria { get; set; }        // agregá este
        public string CategoriaNombre { get; set; } = null!;
        public string? UrlImagen { get; set; }
        public List<ComboProductoDTO> Productos { get; set; } = new();
    }
}