using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public string? TokenRecuperacion { get; set; }

    public DateTime? TokenExpiracion { get; set; }

    public bool Activo { get; set; }

    public int IdRol { get; set; }

    public virtual ICollection<DireccionUsuario> DireccionUsuario { get; set; } = new List<DireccionUsuario>();

    public virtual ICollection<HistorialEstadoPedido> HistorialEstadoPedido { get; set; } = new List<HistorialEstadoPedido>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> PedidoIdClienteNavigation { get; set; } = new List<Pedido>();

    public virtual ICollection<Pedido> PedidoIdEmpleadoNavigation { get; set; } = new List<Pedido>();
}
