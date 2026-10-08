using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

namespace Conexxion
{
    public static class ValidacionDatos
    {
        public const decimal MontoMaximo = 9999999999.99m;
        public static bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            if (email.Length > 100 || email.Any(char.IsWhiteSpace)) return false;
            try
            {
                var direccion = new MailAddress(email);
                string dominio = direccion.Host;
                return direccion.Address == email && dominio.Contains(".") &&
                    !dominio.StartsWith(".") && !dominio.EndsWith(".") && !dominio.Contains("..");
            }
            catch (FormatException) { return false; }
        }
        public static string ErrorCliente(string nombre, string apellido, string dni,
            string telefono, string email, DateTime nacimiento, string plan)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                return "Ingresá nombre y apellido.";
            if (nombre.Trim().Length > 50 || apellido.Trim().Length > 50)
                return "El nombre y el apellido no pueden superar los 50 caracteres.";
            if (string.IsNullOrEmpty(dni) || dni.Length < 7 || dni.Length > 8 || dni.Any(c => c < '0' || c > '9'))
                return "El DNI debe tener entre 7 y 8 dígitos numéricos.";
            if ((telefono ?? "").Trim().Length > 20) return "El teléfono no puede superar los 20 caracteres.";
            if (!EmailValido((email ?? "").Trim())) return "Ingresá un correo electrónico válido.";
            if (nacimiento.Date > DateTime.Today || nacimiento.Year < 1900) return "La fecha de nacimiento ingresada no es válida.";
            if (plan != "3xSemana" && plan != "Full") return "Seleccioná un plan de pago válido.";
            return null;
        }
        public static bool MontoValido(decimal monto)
        {
            return monto > 0 && monto <= MontoMaximo && decimal.Round(monto, 2) == monto;
        }
        public static bool TryMonto(string texto, out decimal monto)
        {
            // Coma decimal es-AR; acepta punto decimal legado, sin separadores de miles ambiguos.
            return decimal.TryParse((texto ?? "").Trim().Replace('.', ','),
                NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("es-AR"), out monto) && MontoValido(monto);
        }
        public static string ErrorPago(decimal monto, string observacion)
        {
            if (!MontoValido(monto)) return "El monto debe ser mayor que cero, tener hasta dos decimales y no superar 9999999999,99.";
            if ((observacion ?? "").Trim().Length > 200) return "La observación no puede superar los 200 caracteres.";
            return null;
        }
    }
}
