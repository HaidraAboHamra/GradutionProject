using GradutionProject.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace GradutionProject.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly string _fromEmail = "workbeilin@gmail.com";
        private readonly string _appPassword = "qehg dbad oufa giun";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_fromEmail, _appPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
