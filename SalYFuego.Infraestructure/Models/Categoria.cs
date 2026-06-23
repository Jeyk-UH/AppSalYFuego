using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Combo> Combo { get; set; } = new List<Combo>();

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}
