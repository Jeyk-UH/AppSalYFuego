using System.Security.Cryptography;
using System.Text;

namespace Sal_Fuego.Aplication.Utils
{
    // Utilidad de encriptación de contraseñas (AES + Hash MD5 para derivar la llave)
    // Buena práctica: comparar contraseñas encriptadas, nunca desencriptar.
    public static class Cryptography
    {
        // Vector de inicialización fijo (16 bytes) usado por el algoritmo AES
        private static readonly byte[] Iv =
        [
            33, 24, 31, 46, 75, 64, 97, 18, 89, 10, 111, 132, 131, 144, 145, 250
        ];

        // Encripta un texto plano (ej. contraseña) usando AES con la llave secreta de la app
        public static string Encrypt(string texto, string secret)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(texto);

            // La llave AES se deriva a partir del hash MD5 de los primeros 32 caracteres del secreto
            string hash = ComputeHash(secret.Substring(0, 32));
            byte[] key = Encoding.UTF8.GetBytes(hash); // 32 bytes

            byte[] encryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = Iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using ICryptoTransform encryptor = aes.CreateEncryptor();
                encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        // Calcula el hash MD5 (en hexadecimal) de un texto
        private static string ComputeHash(string input)
        {
            byte[] bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
