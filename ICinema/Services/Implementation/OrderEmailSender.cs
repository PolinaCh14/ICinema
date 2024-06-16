using ICinema.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ICinema.Services.Implementation;

public class OrderEmailSender : IEmailSender
{
    public async Task SendEmail(string toEmail, string subject, string message)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("ICinema", "icinemaservice888@gmail.com"));
        mailMessage.To.Add(new MailboxAddress("", toEmail));
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart("plain") { Text = message }; ;

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync("icinemaservice888@gmail.com", "rykrrtezrrmxdxmn");
        await client.SendAsync(mailMessage);
        await client.DisconnectAsync(true);
    }
}
