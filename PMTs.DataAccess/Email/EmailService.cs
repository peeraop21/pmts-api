using Microsoft.Extensions.Options;
using MimeKit;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Email.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.DataAccess.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        public EmailService(IOptions<EmailOptions> emailSettingsOptions)
        {
            _options = emailSettingsOptions.Value;
        }

        public async Task SendEmailAsync(EmailRequest email)
        {
            try
            {
                MimeMessage emailMessage = new MimeMessage();

                emailMessage.Subject = email.Subject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = email.Content;
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                for (int i = 0; i < email.From.Count(); i++) if(email.From[i] != null) emailMessage.From.Add(new MailboxAddress(email.From[i], email.From[i]));

                for (int i = 0; i < email.To.Count(); i++) if (email.To[i] != null) emailMessage.To.Add(new MailboxAddress(email.To[i].Split("@")[0], email.To[i]));

                if (email.CC is not null) for (int i = 0; i < email.CC.Count(); i++) if (email.CC[i] != null) emailMessage.Cc.Add(new MailboxAddress(email.CC[i].Split("@")[0], email.CC[i]));
                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_options.Server, _options.Port, MailKit.Security.SecureSocketOptions.None);
                    client.Authenticate(_options.UserName, _options.Password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                string fromStr = string.Join(", ", email.From.Select(s => s));
                string toStr = string.Join(", ", email.To.Select(s => s));
                Console.WriteLine("Error sending email({0}) from {1} to {2} : Error {3}", email.Subject, fromStr, toStr, ex);
            }
        }
    }



}
