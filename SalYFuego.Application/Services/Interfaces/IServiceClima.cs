using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceClima
    {
        // Clima actual en la ubicación del restaurante (San José, Costa Rica).
        // Devuelve null si el Web Service externo no responde.
        Task<ClimaDTO?> ObtenerClimaActualAsync();
    }
}
