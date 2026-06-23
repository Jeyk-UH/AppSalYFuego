using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class DireccionUsuario
{
    public int IdDireccion { get; set; }

    public int IdUsuario { get; set; }

    public string? Alias { get; set; }

    public string? Provincia { get; set; }

    public string? Canton { get; set; }

    public string? Distrito { get; set; }

    public string? DireccionExacta { get; set; }

    public string? Referencia { get; set; }

    public bool EsPredeterminada { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}
