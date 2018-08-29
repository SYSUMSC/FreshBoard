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
                From = new MailAddress("sender email", "sender name"), // TODO: 填写发送者邮箱和名称
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = message.Replace("http://localhost:5000", "destination host"), // TODO: 因为做了反向代理，所以这里要将 localhost 替换成真实域名
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress(email));

            var smtp = new SmtpClient
            {
                Host = "smtp server", // TODO: 发信服务器
                Port = 587,
                EnableSsl = true,
                Credentials =
                    new NetworkCredential("username", "password")
            };

            return smtp.SendMailAsync(msg);
        }
    }
}
