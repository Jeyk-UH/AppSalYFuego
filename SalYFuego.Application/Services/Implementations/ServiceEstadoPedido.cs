using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceEstadoPedido : IServiceEstadoPedido
    {
        private readonly IRepositoryEstadoPedido _repository;

        public ServiceEstadoPedido(IRepositoryEstadoPedido repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<EstadoPedidoDTO>> ListAsync()
        {
            var estados = await _repository.ListAsync();
            return estados.Select(e => new EstadoPedidoDTO
            {
                IdEstado = e.IdEstado,
                Nombre = e.Nombre
            }).ToList();
        }
    }
}
