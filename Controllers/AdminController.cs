using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace mscfreshman.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly IEmailSender _emailSender;

        public AdminController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            DbContextOptions<ApplicationDbContext> dbContextOptions,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContextOptions = dbContextOptions;
            _emailSender = emailSender;
        }

        private async Task<bool> VerifyPrivilege()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return false;
            }

            var user = await _userManager.GetUserAsync(User);
            return user?.Privilege == 1;
        }

        [HttpPost]
        public async Task<IActionResult> GetNotifications(int start = 0, int count = 0)
        {
            if (!await VerifyPrivilege())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                if (count == 0)
                {
                    return Json(db.Notification.OrderByDescending(i => i.Id).Skip(start).ToList());
                }

                return Json(db.Notification.OrderByDescending(i => i.Id).Skip(start).Take(count).ToList());
            }
        }

        /// <summary>
        /// 新建通知
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="nid">消息 Id: 0 -- 新建, 非 0 -- 修改</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NewNotification(string title, string content, DateTime time, int mode, string targets, int nid = 0)
        {
            if (!await VerifyPrivilege())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                if (nid == 0)
                {
                    var notification = new Notification
                    {
                        Content = content,
                        Title = title,
                        Time = time,
                        HasPushed = false,
                        Mode = mode,
                        Targets = targets
                    };

                    await db.Notification.AddAsync(notification);

                    await db.SaveChangesAsync();
                }
                else
                {
                    var notification = await db.Notification.FindAsync(nid);
                    if (notification != null)
                    {
                        notification.Title = title;
                        notification.Content = content;
                        notification.Time = time;
                        notification.Mode = mode;
                        notification.Targets = targets;
                        await db.SaveChangesAsync();
                    }
                }
            }
            return Json(new { succeeded = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNotification(int nid)
        {
            if (!await VerifyPrivilege())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                var notification = await db.Notification.FindAsync(nid);
                if (notification != null)
                {
                    db.Notification.Remove(notification);
                    await db.SaveChangesAsync();
                }
            }
            return Json(new { succeeded = true });
        }

        [HttpPost]
        public async Task<IActionResult> PushNotification(int nid, bool phone, bool email)
        {
            if (!await VerifyPrivilege())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                var notification = await db.Notification.FindAsync(nid);
                if (notification != null)
                {
                    switch (notification.Mode)
                    {
                        case 1:
                            var product = "Dysmsapi";
                            var domain = "dysmsapi.aliyuncs.com";
                            //TODO: Fillin these fields
                            var accessKeyId = "keyId";
                            var accessKeySecret = "keySec";

                            var profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);

                            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
                            IAcsClient acsClient = new DefaultAcsClient(profile);

                            foreach (var user in _userManager.Users)
                            {
                                if (phone && user.PhoneNumberConfirmed)
                                {
                                    SendSmsRequest request = new SendSmsRequest
                                    {
                                        PhoneNumbers = user.PhoneNumber,
                                        SignName = "sysumsc",
                                        TemplateCode = "SMS_143863117",
                                        TemplateParam = JsonConvert.SerializeObject(new { name = user.Name })
                                    };
                                    try
                                    {
                                        var res = acsClient.GetAcsResponse(request);
                                    }
                                    catch
                                    {
                                        //ignored
                                    }
                                }
                                if (email && user.EmailConfirmed)
                                {
                                    try
                                    {
                                        await _emailSender.SendEmailAsync(user.Email, "消息通知 - SYSU MSC", $"<h2>中山大学微软学生俱乐部</h2><p>{user.Name} 您好！您有新的通知，请点击 <a href='{Request.Scheme}://{Request.Host}/Nofication'>此处</a> 查看。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");
                                    }
                                    catch
                                    {
                                        //ignored
                                    }
                                }
                            }
                            break;
                    }
                    notification.HasPushed = true;
                    await db.SaveChangesAsync();
                }
            }
            return Json(new { succeeded = true });
        }
    }
}