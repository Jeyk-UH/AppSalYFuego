using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Combo
{
    public int IdCombo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal PrecioEspecial { get; set; }

    public bool Activo { get; set; }

    public int IdCategoria { get; set; }

    public string? UrlImagen { get; set; }

    public virtual ICollection<ComboProducto> ComboProducto { get; set; } = new List<ComboProducto>();

    public virtual ICollection<DetallePedido> DetallePedido { get; set; } = new List<DetallePedido>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<MenuItem> MenuItem { get; set; } = new List<MenuItem>();
}
