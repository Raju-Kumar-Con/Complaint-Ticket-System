using System.Net;
using System.Net.Mail;

namespace ComplaintTicketSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            using MailMessage mail = new();

            mail.From = new MailAddress(_configuration["EmailSettings:SenderEmail"]!);

            mail.To.Add(toEmail);

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using SmtpClient smtp = new(
                _configuration["EmailSettings:Host"],
                Convert.ToInt32(_configuration["EmailSettings:Port"]));

            smtp.Credentials = new NetworkCredential(
                _configuration["EmailSettings:SenderEmail"],
                _configuration["EmailSettings:SenderPassword"]);

            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mail);
        }
    }
}