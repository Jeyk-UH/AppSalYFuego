using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class HistorialEstacion
{
    public int IdHistorial { get; set; }

    public int IdPedido { get; set; }

    public int IdEstacion { get; set; }

    public DateTime HoraIngreso { get; set; }

    public DateTime? HoraSalida { get; set; }

    public virtual Estacion IdEstacionNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
