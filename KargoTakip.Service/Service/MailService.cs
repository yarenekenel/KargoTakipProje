using KargoTakip.Service.Interface;
using KargoTakip.Service.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace KargoTakip.Service.Service
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendMailAsync(string toMail, string subject, string body)
        {
            var mimeMessage = new MimeMessage();

            var mailboxAddressFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderMail);
            mimeMessage.From.Add(mailboxAddressFrom);

            var mailboxAddressTo = new MailboxAddress("Alıcı", toMail);
            mimeMessage.To.Add(mailboxAddressTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Subject = subject;

            using var client = new SmtpClient();
            await client.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, false);
            await client.AuthenticateAsync(_mailSettings.SenderMail, _mailSettings.AppPassword);
            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
    }
}