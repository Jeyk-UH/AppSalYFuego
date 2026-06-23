using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Estacion
{
    public int IdEstacion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<HistorialEstacion> HistorialEstacion { get; set; } = new List<HistorialEstacion>();

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();

    public virtual ICollection<ProcesoPreparacion> ProcesoPreparacion { get; set; } = new List<ProcesoPreparacion>();
}
