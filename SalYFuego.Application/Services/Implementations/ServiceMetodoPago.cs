using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceMetodoPago : IServiceMetodoPago
    {
        private readonly IRepositoryMetodoPago _repository;

        public ServiceMetodoPago(IRepositoryMetodoPago repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<MetodoPagoDTO>> ListAsync()
        {
            var metodos = await _repository.ListAsync();
            return metodos.Select(m => new MetodoPagoDTO
            {
                IdMetodoPago = m.IdMetodoPago,
                Nombre = m.Nombre
            }).ToList();
        }
    }
}
