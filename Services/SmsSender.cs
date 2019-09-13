using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using qcloudsms_csharp;

namespace FreshBoard.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string phoneNumber, int templateCode, string[] messages);
        Task SendValidationCodeAsync(string phone, string validationCode);
        Task SendNotificationAsync(string phone, string title);
        Task SendStatusChangeAsync(string phone, string changeDescription);
        Task SendStatusChangeAsync(string phone, string periodA, string periodB);
        Task SendStatusChangeAsync(string phone, bool status, string period);
    }

    public class SmsSender : ISmsSender
    {
        /// <summary>
        /// 短信国家代码
        /// </summary>
        private const string NATION_CODE = "86";

        /// <summary>
        /// 短信码号扩展号，格式为纯数字串，其他格式无效。
        /// </summary>
        private const string QCLOUD_EXTEND_CODE = "";

        /// <summary>
        /// 用户的 session 内容，腾讯 server 回包中会原样返回
        /// </summary>
        private const string QCLOUD_EXT = "";

        private readonly SmsOptions _options;
        private readonly SmsSingleSender _singleSender;

        public SmsSender(IOptionsMonitor<SmsOptions> optionsAccessor)
        {
            _options = optionsAccessor.CurrentValue;
            _singleSender = new SmsSingleSender(_options.AppId, _options.AppKey);
        }
        public async Task SendSmsAsync(string phoneNumber, int templateCode, string[] messages)
        {
            var result = await Task.Run(() => _singleSender.sendWithParam(NATION_CODE,
                phoneNumber,
                templateCode,
                messages,
                _options.Signature,
                QCLOUD_EXTEND_CODE,
                QCLOUD_EXT));

            if (result.result != 0)
                throw new SmsException(result.errMsg);
        }

        /// <summary>
        /// 发送短信验证码
        /// 「您的登入验证码为{1}，请在5分钟内填写。」
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="validationCode">验证码，必须为6位以下数字</param>
        /// <returns></returns>
        public Task SendValidationCodeAsync(string phone, string validationCode)
        {
            return SendSmsAsync(phone, _options.Templates.ValidationCode, new[] { validationCode, "5" });
        }

        /// <summary>
        /// 发送通知
        /// 「您有新的通知{1}，请登入网站sysumsc.com查看。回T退订」
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="title">通知标题</param>
        /// <returns></returns>
        public Task SendNotificationAsync(string phone, string title)
        {
            return SendSmsAsync(phone, _options.Templates.Notification, new[] { title });
        }

        /// <summary>
        /// 发送申请状态变更通知
        /// 「您的申请状态更新为{1}，请登入sysumsc.com查看详情。」
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="changeDescription">变更说明</param>
        /// <returns></returns>
        public Task SendStatusChangeAsync(string phone, string changeDescription)
        {
            return SendSmsAsync(phone, _options.Templates.StatusChanged, new[] { changeDescription });
        }

        /// <summary>
        /// 发送申请阶段变更通知
        /// 「您的申请状态更新为通过{periodA}，等待{periodB}，请登入sysumsc.com查看详情。」
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="periodA">源阶段</param>
        /// <param name="periodB">新阶段</param>
        /// <returns></returns>
        public Task SendStatusChangeAsync(string phone, string periodA, string periodB)
        {
            return SendStatusChangeAsync(phone, $"通过{periodA}，等待{periodB}");
        }

        /// <summary>
        /// 发送申请结果通知
        /// 「您的申请状态更新为{period}失败，请登入sysumsc.com查看详情。」
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="status">必须为false</param>
        /// <param name="period">阶段</param>
        /// <returns></returns>
        public Task SendStatusChangeAsync(string phone, bool status, string period)
        {
            if (status != false)
                throw new NotSupportedException();
            return SendStatusChangeAsync(phone, $"{period}失败");
        }
    }
}
