using System;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace Conexxion
{
    public static class EmailService
    {
        public static string ObtenerOrigenRecuperacion(HttpRequest request)
        {
            string configurado = Environment.GetEnvironmentVariable("GYM_PUBLIC_BASE_URL");
            Uri origen;
            if (!string.IsNullOrWhiteSpace(configurado) && Uri.TryCreate(configurado, UriKind.Absolute, out origen) &&
                (origen.Scheme == Uri.UriSchemeHttps || (origen.IsLoopback && origen.Scheme == Uri.UriSchemeHttp)) &&
                string.IsNullOrEmpty(origen.UserInfo) && string.IsNullOrEmpty(origen.Query) && string.IsNullOrEmpty(origen.Fragment))
                return origen.GetLeftPart(UriPartial.Authority);
            if (request.IsLocal && request.Url.IsLoopback)
                return request.Url.GetLeftPart(UriPartial.Authority);
            throw new InvalidOperationException("El origen público de recuperación no está configurado.");
        }

        public static void EnviarRecuperacionPassword(
            string destinatario,
            string nombreUsuario,
            string enlaceRecuperacion)
        {
            using (MailMessage mensaje = new MailMessage())
            {
                mensaje.To.Add(destinatario);

                mensaje.Subject =
                    "Recuperar contraseña - Gym Manager";

                mensaje.IsBodyHtml = true;

                mensaje.Body = $@"
                    <div style='font-family: Arial, sans-serif;
                                max-width: 600px;
                                margin: auto;'>

                        <h2 style='color:#22c55e;'>
                            Gym Manager
                        </h2>

                        <p>
                            Hola {HttpUtility.HtmlEncode(nombreUsuario)},
                        </p>

                        <p>
                            Recibimos una solicitud para
                            restablecer la contraseña de tu cuenta.
                        </p>

                        <p style='margin:30px 0;'>
                            <a href='{HttpUtility.HtmlAttributeEncode(enlaceRecuperacion)}'
                               style='
                                    background:#22c55e;
                                    color:white;
                                    padding:12px 20px;
                                    text-decoration:none;
                                    border-radius:6px;
                                    display:inline-block;'>

                                Cambiar contraseña

                            </a>
                        </p>

                        <p>
                            Este enlace vence en 30 minutos.
                        </p>

                        <p>
                            Si no solicitaste este cambio,
                            simplemente ignorá este correo.
                        </p>

                    </div>";

                using (SmtpClient smtp = new SmtpClient())
                {
                    // Lee automáticamente la configuración
                    // del Web.config
                    string usuario = Environment.GetEnvironmentVariable("GYM_SMTP_USER");
                    string secreto = Environment.GetEnvironmentVariable("GYM_SMTP_PASSWORD");
                    if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(secreto))
                        throw new InvalidOperationException("El servicio de correo no está configurado.");
                    smtp.Credentials = new NetworkCredential(usuario, secreto);
                    smtp.Send(mensaje);
                }
            }
        }
    }
}