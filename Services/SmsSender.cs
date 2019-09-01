using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using System.Threading.Tasks;

namespace mscfreshman.Services
{
    public interface ISmsSender
    {
        Task<SendSmsResponse> SendSmsAsync(string phoneNumber, string message, string templateCode);
    }

    public class SmsSender : ISmsSender
    {
        public Task<SendSmsResponse> SendSmsAsync(string phoneNumber, string message, string templateCode)
        {
            var product = "Dysmsapi";
            var domain = "dysmsapi.aliyuncs.com";

            var accessKeyId = Secrets.AliyunAccessKeyId;
            var accessKeySecret = Secrets.AliyunAccessKeySecret;

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
