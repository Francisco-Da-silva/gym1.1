using System;
using System.Security.Cryptography;
using System.Text;

namespace Conexxion
{
    public static class TokenHelper
    {
        public static string GenerarToken()
        {
            byte[] bytes = new byte[32];

            using (RandomNumberGenerator rng =
                   RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Convert
                .ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public static string ObtenerHash(string token)
        {
            using (SHA256 sha =
                   SHA256.Create())
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(token);

                byte[] hash =
                    sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }
    }
}