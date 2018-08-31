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
                From = new MailAddress("services@sysums.club", "中山大学微软学生俱乐部"),
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = message.Replace("http://localhost:5000", "https://sysums.club")
                    .Replace("https://localhost:5001", "https://sysums.club"),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress(email));

            var smtp = new SmtpClient
            {
                Host = "in-v3.mailjet.com",
                Port = 587,
                EnableSsl = true,
                Credentials =
                    new NetworkCredential("username", "password") //TODO
            };

            return smtp.SendMailAsync(msg);
        }
    }
}
