using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.ExternalServices.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    // Consume el Web Service externo Open-Meteo (a través de IOpenMeteoClient,
    // en la capa de Infraestructura) y traduce el resultado a un DTO listo para
    // mostrar en pantalla.
    public class ServiceClima : IServiceClima
    {
        // Ubicación fija del restaurante (San José, Costa Rica).
        private const double LatitudRestaurante = 9.9281;
        private const double LongitudRestaurante = -84.0907;

        private readonly IOpenMeteoClient _cliente;

        public ServiceClima(IOpenMeteoClient cliente)
        {
            _cliente = cliente;
        }

        public async Task<ClimaDTO?> ObtenerClimaActualAsync()
        {
            var resultado = await _cliente.ObtenerClimaActualAsync(LatitudRestaurante, LongitudRestaurante);
            if (resultado == null)
                return null;

            return new ClimaDTO
            {
                TemperaturaC = resultado.TemperaturaC,
                Descripcion = DescribirCodigoClima(resultado.CodigoClima),
                VientoKmh = resultado.VientoKmh
            };
        }

        // Traducción de los códigos WMO que usa Open-Meteo
        // (ver https://open-meteo.com/en/docs, tabla "WMO Weather interpretation codes").
        private static string DescribirCodigoClima(int codigo) => codigo switch
        {
            0 => "Despejado",
            1 or 2 or 3 => "Parcialmente nublado",
            45 or 48 => "Niebla",
            51 or 53 or 55 => "Llovizna",
            56 or 57 => "Llovizna helada",
            61 or 63 or 65 => "Lluvia",
            66 or 67 => "Lluvia helada",
            71 or 73 or 75 => "Nieve",
            77 => "Granizo",
            80 or 81 or 82 => "Chubascos",
            85 or 86 => "Chubascos de nieve",
            95 => "Tormenta eléctrica",
            96 or 99 => "Tormenta con granizo",
            _ => "Sin datos"
        };
    }
}
