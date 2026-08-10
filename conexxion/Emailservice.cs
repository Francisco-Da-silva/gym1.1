using System;
using System.Net.Mail;

namespace Conexcion
{
    public static class EmailService
    {
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
                            Hola {nombreUsuario},
                        </p>

                        <p>
                            Recibimos una solicitud para
                            restablecer la contraseña de tu cuenta.
                        </p>

                        <p style='margin:30px 0;'>
                            <a href='{enlaceRecuperacion}'
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
                    smtp.Send(mensaje);
                }
            }
        }
    }
}