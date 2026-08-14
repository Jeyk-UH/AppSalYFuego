using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServicePedido : IServicePedido
    {
        private const int TopProductosMasVendidos = 5;

        private readonly IRepositoryPedido _repository;

        public ServicePedido(IRepositoryPedido repository)
        {
            _repository = repository;
        }

        public async Task<EstadisticasAdminDTO> ObtenerEstadisticasAdminAsync()
        {
            var cantidadPedidos = await _repository.ContarPedidosAsync();
            var masVendidos = await _repository.ObtenerProductosMasVendidosAsync(TopProductosMasVendidos);

            return new EstadisticasAdminDTO
            {
                CantidadPedidos = cantidadPedidos,
                ProductosMasVendidos = masVendidos
                    .Select(p => new ProductoVendidoDTO
                    {
                        Nombre = p.Nombre,
                        CantidadVendida = p.CantidadVendida
                    })
                    .ToList()
            };
        }
    }
}
