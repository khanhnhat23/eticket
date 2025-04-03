using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
namespace eTickets.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> LoadEmailTemplateAsync(string templatePath, string name, string link)
        {
            string content = await File.ReadAllTextAsync(templatePath);
            content = content.Replace("{{name}}", name);
            content = content.Replace("{{link}}", link);
            return content;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            // Kiểm tra biến cấu hình
            var senderEmail = _config["Smtp:SenderEmail"];
            var smtpServer = _config["Smtp:Server"];
            var smtpPort = _config["Smtp:Port"];
            var smtpPassword = _config["Smtp:SenderPassword"];

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(smtpServer) ||
                string.IsNullOrEmpty(smtpPort) || string.IsNullOrEmpty(smtpPassword))
            {
                throw new Exception("Cấu hình SMTP bị thiếu hoặc null. Kiểm tra appsettings.json!");
            }

            if (string.IsNullOrEmpty(toEmail))
            {
                throw new ArgumentNullException(nameof(toEmail), "Email người nhận không được để trống!");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Admin", senderEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(smtpServer, int.Parse(smtpPort), SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(senderEmail, smtpPassword);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi gửi email: {ex.Message}");
                }
            }
        }

    }
}
