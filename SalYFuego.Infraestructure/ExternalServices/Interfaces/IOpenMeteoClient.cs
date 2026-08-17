namespace Sal_Fuego.Infraestructure.ExternalServices.Interfaces
{
    // Resultado crudo obtenido de la API pública de Open-Meteo (sin API key).
    public record ClimaActualResultado
    {
        public double TemperaturaC { get; set; }
        public int CodigoClima { get; set; }
        public double VientoKmh { get; set; }
    }

    // Cliente del Web Service externo Open-Meteo (https://open-meteo.com).
    // No requiere API key ni registro.
    public interface IOpenMeteoClient
    {
        // Devuelve null si el servicio externo no responde o hay un error de red;
        // así una falla externa nunca tumba la pantalla que lo consume.
        Task<ClimaActualResultado?> ObtenerClimaActualAsync(double latitud, double longitud);
    }
}
