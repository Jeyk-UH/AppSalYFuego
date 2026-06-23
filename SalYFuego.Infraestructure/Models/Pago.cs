using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal Vuelto { get; set; }

    public string? TipoTarjeta { get; set; }

    public string? UltimosDigitos { get; set; }

    public DateTime FechaPago { get; set; }

    public int IdPedido { get; set; }

    public int IdMetodoPago { get; set; }

    public virtual MetodoPago IdMetodoPagoNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
