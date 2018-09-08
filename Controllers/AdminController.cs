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
using System.Collections.Generic;
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

        private async Task<bool> VerifyPrivilegeAsync()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return false;
            }

            var user = await _userManager.GetUserAsync(User);
            return user?.Privilege == 1;
        }

        [HttpPost]
        public async Task<IActionResult> ModifyPrivilegeAsync(string userId, int privilege)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "没有找到该用户" });
            }
            var thisUser = await _userManager.GetUserAsync(User);
            if (thisUser.Id == user.Id)
            {
                return Json(new { succeeded = false, message = "可别丢人了，不能修改自己的权限" });
            }

            user.Privilege = privilege;
            await _userManager.UpdateAsync(user);
            return Json(new { succeeded = true });
        }

        [HttpPost]
        public async Task<IActionResult> ModifyApplyStatusAsync(string userId, int status)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "没有找到该用户" });
            }

            if (user.ApplyStatus != status)
            {
                user.ApplyStatus = status;
                await _userManager.UpdateAsync(user);

                if (user.PhoneNumberConfirmed)
                {
                    var product = "Dysmsapi";
                    var domain = "dysmsapi.aliyuncs.com";
                    //TODO: Fillin these fields
                    var accessKeyId = "keyId";
                    var accessKeySecret = "keySec";

                    var profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);

                    DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
                    var acsClient = new DefaultAcsClient(profile);

                    var request = new SendSmsRequest
                    {
                        PhoneNumbers = user.PhoneNumber,
                        SignName = "sysumsc",
                        TemplateCode = "SMS_143863265",
                        TemplateParam = JsonConvert.SerializeObject(
                            new
                            {
                                name = user.Name,
                                department = user.Department == 1 ? "行政策划部" : user.Department == 2 ? "媒体宣传部" : user.Department == 3 ? "综合技术部" : "暂无",
                                status = user.ApplyStatus == 1 ? "等待一面" : user.ApplyStatus == 2 ? "等待二面" : user.ApplyStatus == 3 ? "录取失败" : user.ApplyStatus == 4 ? "录取成功" : "暂无"
                            })
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

                if (user.EmailConfirmed)
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(user.Email, "录取状态更新通知 - SYSU MSC", $"<h2>中山大学微软学生俱乐部</h2><p>{user.Name} 您好！您申请 {(user.Department == 1 ? "行政策划部" : user.Department == 2 ? "媒体宣传部" : user.Department == 3 ? "综合技术部" : "暂无")} 的录取状态更新为 {(user.ApplyStatus == 1 ? "等待一面" : user.ApplyStatus == 2 ? "等待二面" : user.ApplyStatus == 3 ? "录取失败" : user.ApplyStatus == 4 ? "录取成功" : "暂无")}，请点击 <a href='{Request.Scheme}://{Request.Host}/Account/Portal'>此处</a> 查看。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");
                    }
                    catch
                    {
                        //ignored
                    }
                }
            }

            return Json(new { succeeded = true });
        }

        [HttpPost]
        public async Task<IActionResult> SearchUsersAsync(string patterns)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            var userList = new List<FreshBoardUser>();
            if (patterns == "$All")
            {
                userList = _userManager.Users.ToList();
            }

            foreach (var user in _userManager.Users.Where(i => i.Id.Contains(patterns)))
            {
                if (!userList.Any(i => i.Id == user.Id))
                {
                    userList.Add(user);
                }
            }
            foreach (var user in _userManager.Users.Where(i => i.Name.Contains(patterns)))
            {
                if (!userList.Any(i => i.Id == user.Id))
                {
                    userList.Add(user);
                }
            }
            foreach (var user in _userManager.Users.Where(i => i.Email.Contains(patterns)))
            {
                if (!userList.Any(i => i.Id == user.Id))
                {
                    userList.Add(user);
                }
            }
            foreach (var user in _userManager.Users.Where(i => i.PhoneNumber.Contains(patterns)))
            {
                if (!userList.Any(i => i.Id == user.Id))
                {
                    userList.Add(user);
                }
            }
            return Json(new
            {
                succeeded = true,
                users = userList.Select(i => new
                {
                    i.Name,
                    i.Email,
                    i.PhoneNumber,
                    i.Department,
                    i.Id,
                    i.EmailConfirmed,
                    i.PhoneNumberConfirmed,
                    i.Sexual,
                    i.CrackProgress,
                    i.ApplyStatus
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetNotificationPushStatusAsync(int nid)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                var notification = await db.Notification.FindAsync(nid);
                if (notification != null)
                {
                    notification.HasPushed = true;
                    await db.SaveChangesAsync();
                }
            }
            return Json(new { succeeded = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationsAsync(int start = 0, int count = 0)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                if (count == 0)
                {
                    return Json(db.Notification.OrderByDescending(i => i.Id).Skip(start).ToList());
                }

                return Json(new { succeeded = true, notifications = db.Notification.OrderByDescending(i => i.Id).Skip(start).Take(count).ToList() });
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
        public async Task<IActionResult> NewNotificationAsync(string title, string content, DateTime time, int mode, string targets, int nid = 0)
        {
            if (!await VerifyPrivilegeAsync())
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
                        Mode = mode,
                        HasPushed = false,
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
                        nid = notification.Id;
                    }
                }
            }
            return Json(new { succeeded = true, nid });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNotificationAsync(int nid)
        {
            if (!await VerifyPrivilegeAsync())
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
        public async Task<IActionResult> PushNotificationAsync(int nid, string userId, bool phone, bool email)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Json(new { succeeded = false, message = "没有找到该用户" });
            }

            var (emailSucceeded, phoneSucceeded) = (false, false);

            if (phone && user.PhoneNumberConfirmed)
            {
                var product = "Dysmsapi";
                var domain = "dysmsapi.aliyuncs.com";
                //TODO: Fillin these fields
                var accessKeyId = "keyId";
                var accessKeySecret = "keySec";

                var profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);

                DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
                var acsClient = new DefaultAcsClient(profile);

                var request = new SendSmsRequest
                {
                    PhoneNumbers = user.PhoneNumber,
                    SignName = "sysumsc",
                    TemplateCode = "SMS_143863117",
                    TemplateParam = JsonConvert.SerializeObject(new { name = user.Name })
                };
                try
                {
                    var res = acsClient.GetAcsResponse(request);
                    phoneSucceeded = res.Code.ToUpper() == "OK";
                }
                catch
                {
                    phoneSucceeded = false;
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
                    emailSucceeded = false;
                }
            }
            return Json(new { succeeded = true, emailSucceeded, phoneSucceeded });
        }

        private class PushUsers
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public bool EmailConfirmed { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public int Department { get; set; }
            public int Sexual { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GetPushNotificationUsersAsync(int nid)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            var usersList = new List<PushUsers>();
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                var notification = await db.Notification.FindAsync(nid);
                if (notification != null)
                {
                    var targets = notification.Targets?.Split("|", StringSplitOptions.RemoveEmptyEntries)?.ToList();

                    IQueryable<FreshBoardUser> users = null;
                    switch (notification.Mode)
                    {
                        case 1:
                            users = _userManager.Users;
                            break;
                        case 2:
                            users = targets == null ? null : _userManager.Users.Where(i => targets.Contains(i.Department.ToString()));
                            break;
                        case 3:
                            users = targets == null ? null : _userManager.Users.Where(i => targets.Contains(i.Id.ToString()));
                            break;
                        case 4:
                            users = targets == null ? null : _userManager.Users.Where(i => targets.Contains(i.Privilege.ToString()));
                            break;
                    }
                    if (users != null)
                    {
                        usersList = users
                        .Select(i => new PushUsers
                        {
                            Email = i.Email,
                            EmailConfirmed = i.EmailConfirmed,
                            PhoneNumber = i.PhoneNumber,
                            PhoneNumberConfirmed = i.PhoneNumberConfirmed,
                            Name = i.Name,
                            Id = i.Id,
                            Department = i.Department,
                            Sexual = i.Sexual
                        }).ToList();
                    }
                }
            }
            return Json(new { succeeded = true, users = usersList });
        }

        [HttpGet]
        public async Task<IActionResult> GetProblemsAsync(int start = 0, int count = 0)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                if (count == 0)
                {
                    return Json(new { succeeded = true, problems = await db.Problem.Skip(start).ToListAsync() });
                }
                else
                {
                    return Json(new { succeeded = true, problems = await db.Problem.Skip(start).Take(count).ToListAsync() });
                }
            }
        }


        /// <summary>
        /// 新建题目
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="level">难度</param>
        /// <param name="pid">为 0 添加，非 0 修改</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NewProblemAsync(string title, string content, string script, int level, int pid = 0)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                if (pid == 0)
                {
                    var problem = new Problem
                    {
                        Title = title,
                        Content = content,
                        Script = script,
                        Level = level
                    };
                    db.Problem.Add(problem);
                }
                else
                {
                    var problem = await db.Problem.FindAsync(pid);
                    if (problem != null)
                    {
                        problem.Title = title;
                        problem.Content = content;
                        problem.Script = script;
                        problem.Level = level;
                    }
                }
                await db.SaveChangesAsync();
            }
            return Json(new { succeeded = true, pid });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProblemAsync(int pid)
        {
            if (!await VerifyPrivilegeAsync())
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                var problem = await db.Problem.FindAsync(pid);
                if (problem != null)
                {
                    db.Problem.Remove(problem);
                }
                await db.SaveChangesAsync();
            }
            return Json(new { succeeded = true });
        }
    }
}