using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class MenuItem
{
    public int IdMenuItem { get; set; }

    public int IdMenu { get; set; }

    public int? IdProducto { get; set; }

    public int? IdCombo { get; set; }

    public virtual Combo? IdComboNavigation { get; set; }

    public virtual Menu IdMenuNavigation { get; set; } = null!;

    public virtual Producto? IdProductoNavigation { get; set; }
}
