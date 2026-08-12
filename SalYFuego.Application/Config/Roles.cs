namespace Sal_Fuego.Aplication.Config
{
    // Nombres de rol usados en [Authorize(Roles = ...)] y en las vistas (User.IsInRole).
    // Deben coincidir exactamente con NombreRol en la tabla ROL.
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Encargado = "Encargado";
        public const string Cocina = "Cocina";
        public const string Cliente = "Cliente";
        public const string Salonero = "Salonero";
        public const string Repartidor = "Repartidor";
    }
}
