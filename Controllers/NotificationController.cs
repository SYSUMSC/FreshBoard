using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using mscfreshman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mscfreshman.Controllers
{
    public class NotificationController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        public NotificationController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _dbContextOptions = dbContextOptions;
        }

        [HttpPost]
        public async Task<IActionResult> DismissNotificationAsync(int nid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(null);
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                if (!db.ReadStatus.Any(i => i.UserId == user.Id && i.NotificationId == nid))
                    db.ReadStatus.Add(new ReadStatus { UserId = user.Id, NotificationId = nid });
                await db.SaveChangesAsync();
            }
            return Json(null);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationsAsync(int start = 0, int count = 0)
        {
            var notifications = new List<NotificationModel>();
            var cnt = 0;
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id ?? string.Empty;
            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                foreach (var i in db.Notification.OrderByDescending(j => j.Id))
                {
                    if (count != 0 && notifications.Count > count) break;
                    switch (i.Mode)
                    {
                        case 1:
                            cnt++;
                            if (cnt > start)
                            {
                                notifications.Add(new NotificationModel
                                {
                                    Id = i.Id,
                                    Title = i.Title,
                                    Content = i.Content,
                                    Preview = GeneratePreview(i.Content),
                                    Time = i.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                                    HasRead = string.IsNullOrEmpty(userId) ? true : await db.ReadStatus.AnyAsync(j => j.NotificationId == i.Id && j.UserId == userId)
                                });
                            }
                            break;
                        case 2:
                            if (user == null) continue;
                            if (i.Targets?.Split("|", StringSplitOptions.RemoveEmptyEntries)?.Contains(user.Department.ToString()) ?? false)
                            {
                                cnt++;
                                if (cnt > start)
                                {
                                    notifications.Add(new NotificationModel
                                    {
                                        Id = i.Id,
                                        Title = i.Title,
                                        Content = i.Content,
                                        Preview = GeneratePreview(i.Content),
                                        Time = i.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                                        HasRead = string.IsNullOrEmpty(userId) ? true : await db.ReadStatus.AnyAsync(j => j.NotificationId == i.Id && j.UserId == userId)
                                    });
                                }
                            }
                            break;
                        case 3:
                            if (user == null) continue;
                            if (i.Targets?.Split("|", StringSplitOptions.RemoveEmptyEntries)?.Contains(user.Id) ?? false)
                            {
                                cnt++;
                                if (cnt > start)
                                {
                                    notifications.Add(new NotificationModel
                                    {
                                        Id = i.Id,
                                        Title = i.Title,
                                        Content = i.Content,
                                        Preview = GeneratePreview(i.Content),
                                        Time = i.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                                        HasRead = string.IsNullOrEmpty(userId) ? true : await db.ReadStatus.AnyAsync(j => j.NotificationId == i.Id && j.UserId == userId)
                                    });
                                }
                            }
                            break;
                        case 4:
                            if (user == null) continue;
                            if (i.Targets?.Split("|", StringSplitOptions.RemoveEmptyEntries)?.Contains(user.Privilege.ToString()) ?? false)
                            {
                                cnt++;
                                if (cnt > start)
                                {
                                    notifications.Add(new NotificationModel
                                    {
                                        Id = i.Id,
                                        Title = i.Title,
                                        Content = i.Content,
                                        Preview = GeneratePreview(i.Content),
                                        Time = i.Time.ToString("yyyy/MM/dd HH:mm:ss"),
                                        HasRead = string.IsNullOrEmpty(userId) ? true : await db.ReadStatus.AnyAsync(j => j.NotificationId == i.Id && j.UserId == userId)
                                    });
                                }
                            }
                            break;
                    }
                }
            }
            return Json(notifications);
        }

        private string GeneratePreview(string content)
        {
            if (content == null) return null;
            var scriptreg = new Regex("(?i)(<SCRIPT)[\\s\\S]*?((</SCRIPT>)|(/>))");
            content = scriptreg.Replace(content, string.Empty);
            var blankreg = new Regex("\\s+|\t|\r|\n");
            content = blankreg.Replace(content, " ");
            var htmlreg = new Regex("<[^>]*>|");
            content = htmlreg.Replace(content, string.Empty);
            return content.Substring(0, Math.Min(content.Length, 450));
        }
    }
}