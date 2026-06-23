namespace Sal_Fuego.Aplication.DTOs
{
    public record IngredienteDTO
    {
        public int IdIngrediente { get; set; }
        public string Nombre { get; set; } = null!;
       
    }
}