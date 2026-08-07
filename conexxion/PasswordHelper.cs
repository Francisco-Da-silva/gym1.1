using System;
using System.Security.Cryptography;

namespace Conexxion
{
    public static class PasswordHelper
    {
        private const int Iteraciones = 100000;
        private const int TamanoSalt = 16;
        private const int TamanoHash = 32;

        public static string CrearSalt()
        {
            byte[] salt = new byte[TamanoSalt];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public static string CrearHash(string password, string saltBase64)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("La contraseña es obligatoria.");
            }

            byte[] salt = Convert.FromBase64String(saltBase64);

            using (Rfc2898DeriveBytes pbkdf2 =
                   new Rfc2898DeriveBytes(
                       password,
                       salt,
                       Iteraciones,
                       HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(TamanoHash);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerificarPassword(
            string password,
            string saltBase64,
            string hashGuardado)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(saltBase64) ||
                string.IsNullOrWhiteSpace(hashGuardado))
            {
                return false;
            }

            byte[] hashCalculado =
                Convert.FromBase64String(
                    CrearHash(password, saltBase64)
                );

            byte[] hashOriginal =
                Convert.FromBase64String(hashGuardado);

            if (hashCalculado.Length != hashOriginal.Length)
            {
                return false;
            }

            int diferencia = 0;

            for (int i = 0; i < hashCalculado.Length; i++)
            {
                diferencia |= hashCalculado[i] ^ hashOriginal[i];
            }

            return diferencia == 0;
        }
    }
}
