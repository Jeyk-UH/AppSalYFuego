namespace Sal_Fuego.Aplication.DTOs
{
    public record CategoriaDTO
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = null!;
    }
}