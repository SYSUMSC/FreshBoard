using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace mscfreshman.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
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

            var smtp = new SmtpClient
            {
                Host = "smtpdm.aliyun.com",
                Port = 25,
                Credentials =
                    new NetworkCredential("services@sysumsc.com", Secrets.EmailPassword)
            };

            return smtp.SendMailAsync(msg);
        }
    }
}
