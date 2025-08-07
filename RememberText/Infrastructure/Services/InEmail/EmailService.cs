using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InEmail
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _Configuration;
        private readonly ILogger<EmailService> _Logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _Configuration = configuration;
            _Logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var from = _Configuration["EmailPropertyes:FromMail"];
            var pass = _Configuration["EmailPropertyes:PassMail"];
            var host = _Configuration["EmailPropertyes:HostMail"];
            int port = Convert.ToInt32(_Configuration["EmailPropertyes:PortMail"]);
            bool enableSsl = _Configuration.GetValue<bool>("EmailPropertyes:EnableSsl");

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Remembert Ext", from));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            //dnvo ppbi vejj oqjm
            using (var client = new SmtpClient())
            {
                CancellationToken ct = default;
                try
                {
                    await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls, ct);
                    await client.AuthenticateAsync(from, pass, ct);
                    await client.SendAsync(emailMessage, ct);
                }
                catch (Exception ex)
                {
                    _Logger.LogError(ex, $"SendEmail error {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true, ct);
                    client.Dispose();
                }
            }
        }
    }
}
