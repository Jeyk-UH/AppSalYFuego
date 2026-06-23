using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class DetallePedido
{
    public int IdDetalle { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public string? Observaciones { get; set; }

    public int IdPedido { get; set; }

    public int? IdProducto { get; set; }

    public int? IdCombo { get; set; }

    public virtual Combo? IdComboNavigation { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Producto? IdProductoNavigation { get; set; }
}
