using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using mscfreshman.Data.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mscfreshman.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        private class OtherInfoList
        {
            public string Key { get; set; }
            public string Description { get; set; }
            public string Value { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            var data = new
            {
                isSignedIn = _signInManager.IsSignedIn(User),
                userInfo = await _userManager.GetUserAsync(User)
            };
            bool flag = true;
            if (data.userInfo == null && data.isSignedIn)
            {
                await _signInManager.SignOutAsync();
                flag = false;
            }

            var otherInfo = new OtherInfoTemplate();
            if (data.userInfo != null && !string.IsNullOrEmpty(data.userInfo.OtherInfo))
            {
                otherInfo = JsonConvert.DeserializeObject<OtherInfoTemplate>(data.userInfo.OtherInfo);
            }

            var properties = otherInfo.GetType().GetProperties();
            var otherInfoList = new List<OtherInfoList>();
            foreach (var property in properties)
            {
                if (!property.IsDefined(typeof(NameAttribute), false))
                {
                    continue;
                }

                var attributes = property.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute.GetType().Name == "NameAttribute")
                    {
                        otherInfoList.Add(new OtherInfoList
                        {
                            Key = property.Name,
                            Description = attribute.GetType().GetProperty("Name").GetValue(attribute)?.ToString(),
                            Value = property.GetValue(otherInfo)?.ToString()
                        });
                        break;
                    }
                }
            }

            return Json(new { isSignedIn = data.isSignedIn && flag, data.userInfo, otherInfo = otherInfoList });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string email, string password, bool persistent = true)
        {
            await _signInManager.SignOutAsync();

            if (string.IsNullOrEmpty(email))
            {
                return Json(new { succeeded = false, message = "未填写邮箱" });
            }

            if (string.IsNullOrEmpty(password))
            {
                return Json(new { succeeded = false, message = "未填写密码" });
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, persistent, false);
            if (result.Succeeded)
            {
                return Json(new { succeeded = true });
            }

            return Json(new { succeeded = false, message = "用户名或密码不正确" });
        }

        [HttpPost]
        public async Task<IActionResult> SendRegisterEmailAsync()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Json(new { succeeded = false, message = "未登录" });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "发生未知错误" });
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var email = user.Email;
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, Request.Scheme);

            try
            {
                await _emailSender.SendEmailAsync(email, "验证邮箱 - SYSU MSC", $"<h2>中山大学微软学生俱乐部</h2><p>感谢您的注册，请点击 <a href='{callbackUrl}'>此处</a> 验证你的邮箱地址。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Json(new { succeeded = true });

        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(
            string name, //姓名
            string email, //email
            DateTime dob,
            int grade, //年级
            string phone, //电话
            string qq, //QQ
            string wechat,
            int cpclevel, //政治面貌
            string institute, //学院
            string major, //专业
            int sexual, //性别
            int schnum, //学号
            string password, //密码
            string confirmpassword) // 确认密码
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { succeeded = false, message = "未填写邮箱" });
            }

            if (string.IsNullOrEmpty(password))
            {
                return Json(new { succeeded = false, message = "未填写密码" });
            }

            if (password != confirmpassword)
            {
                return Json(new { succeeded = false, message = "密码和确认密码不匹配" });
            }

            var user = new FreshBoardUser
            {
                UserName = email,
                Email = email,
                Name = name,
                DOB = dob,
                Grade = grade,
                PhoneNumber = phone,
                WeChat = wechat,
                QQ = qq,
                Sexual = sexual,
                CPCLevel = cpclevel,
                Institute = institute,
                Major = major,
                SchoolNumber = schnum,
                Privilege = 0
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, Request.Scheme);
                try
                {
                    await _emailSender.SendEmailAsync(email, "验证邮箱 - SYSU MSC", $"<h2>中山大学微软学生俱乐部</h2><p>感谢您的注册，请点击 <a href='{callbackUrl}'>此处</a> 验证你的邮箱地址。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, false);
                return Json(new { succeeded = true });
            }

            return Json(new { succeeded = false, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "注册失败" });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(userId), code);
            return Json(new { succeeded = result.Succeeded, errors = result.Errors.Select(i => i.Description) });
        }

        [HttpPost]
        public async Task<IActionResult> ModifyOtherAsync(string data)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "未登录" });
            }
            user.OtherInfo = data;

            var result = await _userManager.UpdateAsync(user);
            return Json(new { succeeded = result.Succeeded, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "修改失败" });
        }

        [HttpPost]
        public async Task<IActionResult> ModifyAsync(
            string name, //姓名
            string email, //email
            DateTime dob, //出生日期
            int grade, //年级
            string phone, //电话
            string qq, //QQ
            string wechat,
            int cpclevel, //政治面貌
            string institute, //学院
            string major, //专业
            int sexual, //性别
            int schnum) //学号
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "未登录" });
            }

            user.Name = name;
            user.Email = email;
            user.DOB = dob;
            user.Grade = grade;
            user.PhoneNumber = phone;
            user.QQ = qq;
            user.WeChat = wechat;
            user.CPCLevel = cpclevel;
            user.Institute = institute;
            user.Major = major;
            user.Sexual = sexual;
            user.SchoolNumber = schnum;

            var result = await _userManager.UpdateAsync(user);
            return Json(new { succeeded = result.Succeeded, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "修改失败" });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyAsync(int department)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "未登录" });
            }

            if (user.Department != department)
            {
                user.ApplyStatus = 0;
            }

            user.Department = department;

            var result = await _userManager.UpdateAsync(user);
            return Json(new { succeeded = result.Succeeded, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "申请失败" });
        }
    }
}