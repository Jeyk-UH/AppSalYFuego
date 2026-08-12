namespace Sal_Fuego.Aplication.DTOs
{
    public record ComboProductoFormDTO
    {
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}