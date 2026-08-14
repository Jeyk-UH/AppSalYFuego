namespace Sal_Fuego.Aplication.DTOs
{
    public record ProductoVendidoDTO
    {
        public string Nombre { get; set; } = null!;
        public int CantidadVendida { get; set; }
    }
}
