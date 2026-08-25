namespace Sal_Fuego.Aplication.DTOs
{
    // Un punto del gráfico "Pedidos por día": ya trae la etiqueta lista para
    // mostrar (ej. "Lun 18/08") para no tener que formatear fechas en el JS.
    public record PedidosPorDiaDTO
    {
        public string Etiqueta { get; set; } = null!;
        public int Cantidad { get; set; }
    }

    // Un producto o combo dentro del ranking de más/menos vendidos.
    public record ItemVendidoDTO
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!; // "Producto" o "Combo"
        public int CantidadVendida { get; set; }
    }

    // Todo lo que necesita la pantalla de Reportes del Administrador.
    public record ReporteVentasDTO
    {
        public List<PedidosPorDiaDTO> PedidosPorDia { get; set; } = new();
        public List<ItemVendidoDTO> MasVendidos { get; set; } = new();
        public List<ItemVendidoDTO> MenosVendidos { get; set; } = new();
    }
}
