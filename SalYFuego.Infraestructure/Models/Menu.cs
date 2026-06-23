using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class Menu
{
    public int IdMenu { get; set; }

    public string Nombre { get; set; } = null!;

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public bool EstaActivo { get; set; }

    public virtual ICollection<MenuDisponibilidad> MenuDisponibilidad { get; set; } = new List<MenuDisponibilidad>();

    public virtual ICollection<MenuItem> MenuItem { get; set; } = new List<MenuItem>();
}
