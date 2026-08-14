using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class MetodoPago
{
    public int IdMetodoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
