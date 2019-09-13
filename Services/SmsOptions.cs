public class SmsOptions
{
    /// <summary>
    /// 短信应用 App ID
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// 短信应用 App Key
    /// </summary>
    public string AppKey { get; set; } = "9ff91d87c2cd7cd0ea762f141975d1df37481d48700d70ac37470aefc60f9bad";

    /// <summary>
    /// 短信模板信息
    /// </summary>
    /// <returns></returns>
    public TemplateOption Templates { get; set; } = new TemplateOption();

    /// <summary>
    /// 签名内容
    /// </summary>
    public string Signature { get; set; } = "腾讯云";

    /// <summary>
    /// 短信模板配置
    /// </summary>
    public class TemplateOption
    {
        public TemplateOption()
        {
        }

        public TemplateOption(int validationCode, int notification, int statusChanged)
        {
            ValidationCode = validationCode;
            Notification = notification;
            StatusChanged = statusChanged;
        }

        /// <summary>
        /// 验证码短信模板 ID
        /// </summary>
        public int ValidationCode { get; set; }

        /// <summary>
        /// 通知短信模板 ID
        /// </summary>
        public int Notification { get; set; }

        /// <summary>
        /// 申请状态变更模板 ID
        /// </summary>
        public int StatusChanged { get; set; }
    }
}
