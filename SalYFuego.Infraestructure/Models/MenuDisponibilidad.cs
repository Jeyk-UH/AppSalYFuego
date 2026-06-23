using System;
using System.Collections.Generic;

namespace SalYFuego.Infraestructure.Models;

public partial class MenuDisponibilidad
{
    public int IdDisponibilidad { get; set; }

    public int IdMenu { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public string? DiaSemana { get; set; }

    public virtual Menu IdMenuNavigation { get; set; } = null!;
}
