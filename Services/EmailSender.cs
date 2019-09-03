using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FreshBoard.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var msg = new MailMessage
            {
                From = new MailAddress("services@sysumsc.com", "SYSU MSC", Encoding.UTF8),
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = message.Replace("http://localhost:5000", "https://sysumsc.com")
                    .Replace("https://localhost:5001", "https://sysumsc.com"),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress(email));

            using var smtp = new SmtpClient
            {
                Host = _configuration["Email:HostName"],
                Port = 25,
                Credentials =
                    new NetworkCredential(_configuration["Email:UserName"], _configuration["Email:Password"])
            };

            await smtp.SendMailAsync(msg);
        }
    }
}
