using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Transporte.BL
{
    public class CorreoService
    {
        private const string CorreoEmisor = "transportesticobus@gmail.com";

        private const string ClaveAplicacion = "crlteimnkhukmgqh";

        public async Task EnviarInicioSesion(string correoDestino, string nombreUsuario)
        {
            var mensaje = new MimeMessage();

            mensaje.From.Add(
                new MailboxAddress(
                    "Gestor de Transporte",
                    CorreoEmisor));

            mensaje.To.Add(
                MailboxAddress.Parse(correoDestino));

            mensaje.Subject = "Inicio de sesión detectado";

            mensaje.Body = new TextPart("plain")
            {
                Text =
$@"Hola {nombreUsuario},

Se detectó un inicio de sesión en el sistema Gestor de Transporte.

Fecha y hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}

Si no reconoces esta actividad, comunícate con el administrador.

Este es un correo automático."
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                CorreoEmisor,
                ClaveAplicacion);

            await smtp.SendAsync(mensaje);

            await smtp.DisconnectAsync(true);
        }
    }
}