using System.ComponentModel.DataAnnotations;

public class EmailOptions
{
    /// <summary>
    /// SMTP 发信服务器地址
    /// </summary>
    /// <value></value>
    [Required]
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// SMTP 连接端口
    /// </summary>
    /// <value></value>
    public int Port { get; set; } = 25;

    /// <summary>
    /// SMTP 用户名
    /// </summary>
    /// <value></value>
    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// SMTP 密码
    /// </summary>
    /// <value></value>
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FromAddress { get; set; } = string.Empty;

    [Required]
    public string FromName { get; set; } = string.Empty;
}
