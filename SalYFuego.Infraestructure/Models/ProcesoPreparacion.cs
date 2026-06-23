using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class ProcesoPreparacion
{
    public int IdProceso { get; set; }

    public int OrdenPaso { get; set; }

    public int TiempoEstimadoMinutos { get; set; }

    public int IdProducto { get; set; }

    public int IdEstacion { get; set; }

    public virtual Estacion IdEstacionNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
