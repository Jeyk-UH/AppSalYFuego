using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Sal_Fuego.Infraestructure.ExternalServices.Interfaces;

namespace Sal_Fuego.Infraestructure.ExternalServices.Implementations
{
    // Consume la API pública de Open-Meteo (https://open-meteo.com/en/docs) para
    // obtener el clima actual. Es completamente gratuita y no exige API key.
    public class OpenMeteoClient : IOpenMeteoClient
    {
        private readonly HttpClient _http;

        public OpenMeteoClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ClimaActualResultado?> ObtenerClimaActualAsync(double latitud, double longitud)
        {
            try
            {
                var lat = latitud.ToString(CultureInfo.InvariantCulture);
                var lon = longitud.ToString(CultureInfo.InvariantCulture);
                var url = $"v1/forecast?latitude={lat}&longitude={lon}" +
                           "&current=temperature_2m,weather_code,wind_speed_10m&timezone=America%2FCosta_Rica";

                var respuesta = await _http.GetFromJsonAsync<OpenMeteoRespuesta>(url);
                if (respuesta?.Current == null)
                    return null;

                return new ClimaActualResultado
                {
                    TemperaturaC = respuesta.Current.Temperature2m,
                    CodigoClima = respuesta.Current.WeatherCode,
                    VientoKmh = respuesta.Current.WindSpeed10m
                };
            }
            catch
            {
                // Si Open-Meteo no responde (sin internet, timeout, etc.) se
                // devuelve null en vez de propagar la excepción: el clima es un
                // dato informativo, no debe romper el registro del pedido.
                return null;
            }
        }

        private record OpenMeteoRespuesta
        {
            [JsonPropertyName("current")]
            public OpenMeteoActual? Current { get; set; }
        }

        private record OpenMeteoActual
        {
            [JsonPropertyName("temperature_2m")]
            public double Temperature2m { get; set; }

            [JsonPropertyName("weather_code")]
            public int WeatherCode { get; set; }

            [JsonPropertyName("wind_speed_10m")]
            public double WindSpeed10m { get; set; }
        }
    }
}
