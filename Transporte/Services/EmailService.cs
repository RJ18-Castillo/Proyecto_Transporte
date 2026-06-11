using MailKit.Net.Smtp;
using MimeKit;

namespace Transporte.UI.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
    {
        var settings = _config.GetSection("EmailSettings");

        var mensaje = new MimeMessage();
        mensaje.From.Add(MailboxAddress.Parse(settings["User"]));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;
        mensaje.Body = new TextPart("plain") { Text = cuerpo };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(settings["Host"],
            int.Parse(settings["Port"]!),
            MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(settings["User"], settings["Password"]);
        await smtp.SendAsync(mensaje);
        await smtp.DisconnectAsync(true);
    }
}