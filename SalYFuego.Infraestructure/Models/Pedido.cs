using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public string CodigoOrden { get; set; } = null!;

    public DateTime FechaPedido { get; set; }

    public string OrigenPedido { get; set; } = null!;

    public string MetodoEntrega { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal CostoEnvio { get; set; }

    public decimal Total { get; set; }

    public int IdEstado { get; set; }

    public int? IdEstacionActual { get; set; }

    public int? IdCliente { get; set; }

    public int? IdEmpleado { get; set; }

    public int? IdDireccionEntrega { get; set; }

    // Datos de un cliente sin cuenta (venta anónima desde Caja). Solo se usan
    // cuando IdCliente es null.
    public string? NombreClienteInvitado { get; set; }

    public string? CedulaClienteInvitado { get; set; }

    public virtual ICollection<DetallePedido> DetallePedido { get; set; } = new List<DetallePedido>();

    public virtual ICollection<HistorialEstacion> HistorialEstacion { get; set; } = new List<HistorialEstacion>();

    public virtual ICollection<HistorialEstadoPedido> HistorialEstadoPedido { get; set; } = new List<HistorialEstadoPedido>();

    public virtual Usuario? IdClienteNavigation { get; set; }

    public virtual DireccionUsuario? IdDireccionEntregaNavigation { get; set; }

    public virtual Usuario? IdEmpleadoNavigation { get; set; }

    public virtual Estacion? IdEstacionActualNavigation { get; set; }

    public virtual EstadoPedido IdEstadoNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
