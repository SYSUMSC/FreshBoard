using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FreshBoard.Services
{
    public interface ISmsSender
    {
        Task<SendSmsResponse> SendSmsAsync(string phoneNumber, string message, string templateCode);
    }

    public class SmsSender : ISmsSender
    {
        private readonly IConfiguration _configuration;

        public SmsSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<SendSmsResponse> SendSmsAsync(string phoneNumber, string message, string templateCode)
        {
            var product = "Dysmsapi";
            var domain = _configuration["Sms:HostName"];

            var accessKeyId = _configuration["Sms:UserName"];
            var accessKeySecret = _configuration["Sms:Password"];

            var profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);

            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            var acsClient = new DefaultAcsClient(profile);

            var request = new SendSmsRequest
            {
                PhoneNumbers = phoneNumber,
                SignName = "sysumsc",
                TemplateCode = templateCode,
                TemplateParam = message
            };

            return Task.Run(() => acsClient.GetAcsResponse(request));
        }
    }
}
