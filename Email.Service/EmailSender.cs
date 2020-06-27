using Email.Service.EmailEnvironment;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace Email.Service
{
    /// <summary>
    /// https://kenhaggerty.com/articles/article/aspnet-core-22-smtp-emailsender-implementation
    /// </summary>
    // using IHostingEnvironment = Microsoft.AspNetCore.Hosting.;
    //using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        //private readonly Microsoft.Extensions.Hosting.IHostingEnvironment _env;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        //Microsoft.Extensions.Hosting.IHostingEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            //_env = env;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
                mimeMessage.To.Add(new MailboxAddress(email));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.CheckCertificateRevocation = true;
                   
                    await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, false);

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

                    await client.SendAsync(mimeMessage);

                  //  await client.DisconnectAsync(true);
                }
            }
            catch (System.Exception ex)
            {
                // TODO: handle exception
                throw new System.InvalidOperationException(ex.Message);
            }
        }

    }
}
