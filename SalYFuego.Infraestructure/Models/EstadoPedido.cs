using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class EstadoPedido
{
    public int IdEstado { get; set; }

    public string Nombre { get; set; } = null!;

    public int? Orden { get; set; }

    public virtual ICollection<HistorialEstadoPedido> HistorialEstadoPedido { get; set; } = new List<HistorialEstadoPedido>();

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}
