using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public bool Activo { get; set; }

    public int IdCategoria { get; set; }

    public virtual ICollection<ComboProducto> ComboProducto { get; set; } = new List<ComboProducto>();

    public virtual ICollection<DetallePedido> DetallePedido { get; set; } = new List<DetallePedido>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<MenuItem> MenuItem { get; set; } = new List<MenuItem>();

    public virtual ICollection<ProcesoPreparacion> ProcesoPreparacion { get; set; } = new List<ProcesoPreparacion>();

    public virtual ICollection<ProductoImagen> ProductoImagen { get; set; } = new List<ProductoImagen>();

    public virtual ICollection<Ingrediente> IdIngrediente { get; set; } = new List<Ingrediente>();
}
