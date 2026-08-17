using Sal_Fuego.Aplication.DTOs;

namespace Sal_Fuego.Aplication.Services.Interfaces
{
    public interface IServiceMetodoPago
    {
        Task<ICollection<MetodoPagoDTO>> ListAsync();

        // Métodos válidos para un cobro presencial (Caja, o el botón Cobrar de un
        // pedido pendiente): se paga en efectivo o con tarjeta directo en el
        // datáfono. Excluye "Pago Web" (solo aplica a un pago hecho en línea).
        Task<ICollection<MetodoPagoDTO>> ListPresencialAsync();

        // Métodos válidos para el checkout en línea del Cliente (Carrito):
        // Efectivo (paga al retirar/recibir) o Pago Web (cobro simulado en línea).
        Task<ICollection<MetodoPagoDTO>> ListEnLineaAsync();
    }
}
