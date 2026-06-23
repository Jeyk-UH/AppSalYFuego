using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class HistorialEstadoPedido
{
    public int IdHistorialEstado { get; set; }

    public int IdPedido { get; set; }

    public int IdEstado { get; set; }

    public DateTime FechaHora { get; set; }

    public int IdUsuario { get; set; }

    public string? Observacion { get; set; }

    public virtual EstadoPedido IdEstadoNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
