namespace Sal_Fuego.Aplication.DTOs
{
    public record ComboProductoDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}