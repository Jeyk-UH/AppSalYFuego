using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class ProductoImagen
{
    public int IdImagen { get; set; }

    public int IdProducto { get; set; }

    public string UrlImagen { get; set; } = null!;

    public bool EsPrincipal { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
