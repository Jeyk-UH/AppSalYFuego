namespace Sal_Fuego.Aplication.DTOs
{
    // Clima actual (consumo del Web Service público Open-Meteo), mostrado junto
    // a la entrega a domicilio del Carrito como referencia informativa.
    public record ClimaDTO
    {
        public double TemperaturaC { get; set; }
        public string Descripcion { get; set; } = null!;
        public double VientoKmh { get; set; }
    }
}
