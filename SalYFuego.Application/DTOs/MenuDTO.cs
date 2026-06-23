namespace Sal_Fuego.Aplication.DTOs
{
    public record MenuDTO
    {
        public int IdMenu { get; set; }
        public string Nombre { get; set; } = null!;
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public bool EstaActivo { get; set; }
        // Disponibilidad del menú
        public List<MenuDisponibilidadDTO> Disponibilidad { get; set; } = new();
        // Items del menú (productos y combos)
        public List<MenuItemDTO> Items { get; set; } = new();
    }
}