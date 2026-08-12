namespace Sal_Fuego.Aplication.Config
{
    // Configuración general de la aplicación, mapeada desde appsettings.*.json
    public class AppConfig
    {
        public Crypto Crypto { get; set; } = default!;
    }

    // Llave secreta usada para encriptar/comparar contraseñas
    public class Crypto
    {
        public string Secret { get; set; } = default!;
    }
}
