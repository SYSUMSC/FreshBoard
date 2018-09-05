using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace mscfreshman.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        public Task Execute(string subject, string message, string email)
        {
            var msg = new MailMessage
            {
                From = new MailAddress("services@sysumsc.com", "中山大学微软学生俱乐部", Encoding.UTF8),
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = message.Replace("http://localhost:5000", "https://sysumsc.com")
                    .Replace("https://localhost:5001", "https://sysumsc.com"),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress(email));

            var smtp = new SmtpClient
            {
                Host = "in-v3.mailjet.com",
                Port = 587,
                Credentials =
                    new NetworkCredential("username", "password") //TODO: Fillin
            };

            return smtp.SendMailAsync(msg);
        }
    }
}
