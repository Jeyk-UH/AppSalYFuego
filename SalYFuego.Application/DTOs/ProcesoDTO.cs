namespace Sal_Fuego.Aplication.DTOs
{
    public record ProcesoDTO
    {
        // Nombre del producto
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        // Cantidad total de pasos
        public int CantidadPasos { get; set; }
        // Lista de estaciones en orden
        public List<PasoProcesoDTO> Pasos { get; set; } = new();
    }
}