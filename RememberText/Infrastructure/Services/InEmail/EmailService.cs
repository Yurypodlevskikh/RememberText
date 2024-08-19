using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InEmail
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _Configuration;

        public EmailService(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var from = _Configuration["EmailPropertyes:FromMail"];
            var pass = _Configuration["EmailPropertyes:PassMail"];
            var host = _Configuration["EmailPropertyes:HostMail"];
            var port = Convert.ToInt32(_Configuration["EmailPropertyes:PortMail"]);

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Remembert Ext", from));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, false);
                await client.AuthenticateAsync(from, pass);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
