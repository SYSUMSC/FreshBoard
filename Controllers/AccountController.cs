using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using mscfreshman.Data.Identity;
using mscfreshman.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace mscfreshman.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public AccountController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
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
        public async Task<IActionResult> GetSpecificUserInfoAsync(string userId)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Json(new { succeeded = false, message = "未登录" });
            }

            var admin = await _userManager.GetUserAsync(User);
            if (admin == null)
            {
                return Json(new { succeeded = false, message = "发生未知错误" });
            }
            if (admin.Privilege != 1)
            {
                return Json(new { succeeded = false, message = "没有权限" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Json(new { succeeded = false, message = "没有找到该用户" });
            }

            var data = new
            {
                userInfo = user,
                otherInfo = user.OtherInfo
            };

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

            return Json(new { succeeded = true, data.userInfo, otherInfo = otherInfoList });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            Console.WriteLine(Request.Host);
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
            string phone, //手机
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
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Json(new { succeeded = false, errors = new[] { "找不到该用户" } });
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return Json(new { succeeded = result.Succeeded, errors = result.Errors.Select(i => i.Description) });
        }

        [HttpPost]
        public async Task<IActionResult> SendSMSAsync()
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
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            
            try
            {
                var res = await _smsSender.SendSmsAsync(user.PhoneNumber, JsonConvert.SerializeObject(new { code = token }), "SMS_143868088");
                return Json(new { succeeded = res.Code.ToUpper() == "OK", message = res.Message });
            }
            catch
            {
                return Json(new { succeeded = false, message = "发生未知错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPhoneAsync(string token)
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
            var res = await _userManager.VerifyChangePhoneNumberTokenAsync(user, token, user.PhoneNumber);
            user.PhoneNumberConfirmed = res;
            await _userManager.UpdateAsync(user);
            return Json(new { succeeded = res });
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
            string phone, //手机
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
            var emailChanged = false;
            user.Name = name;
            if (user.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, email);
                if (!setEmailResult.Succeeded)
                {
                    return Json(new { succeeded = false, message = "电子邮件修改失败" });
                }
                emailChanged = true;
            }
            user.UserName = email;
            user.DOB = dob;
            user.Grade = grade;
            if (user.PhoneNumber != phone)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, phone);
                if (!setPhoneResult.Succeeded)
                {
                    return Json(new { succeeded = false, message = "手机号码修改失败" });
                }
            }
            user.QQ = qq;
            user.WeChat = wechat;
            user.CPCLevel = cpclevel;
            user.Institute = institute;
            user.Major = major;
            user.Sexual = sexual;
            user.SchoolNumber = schnum;

            var result = await _userManager.UpdateAsync(user);
            if (emailChanged)
            {
                await _signInManager.RefreshSignInAsync(user);
            }
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
                if (department == 0)
                {
                    user.ApplyStatus = 0;
                }
                else
                {
                    user.ApplyStatus = user.CrackProgress == 10 ? 2 : 1;
                }
            }

            user.Department = department;

            var result = await _userManager.UpdateAsync(user);
            return Json(new { succeeded = result.Succeeded, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "申请失败" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountAsync(string password)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { succeeded = false, message = "未登录" });
            }
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return Json(new { succeeded = false, message = "密码不正确" });
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
            }

            return Json(new { succeeded = result.Succeeded, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "删除失败" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(string userId, string token, string password, string confirmpassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Json(new { succeeded = false, message = "重置失败" });
            if (password != confirmpassword) return Json(new { succeeded = false, message = "密码和确认密码不匹配" });
            if (!string.IsNullOrEmpty(token)) token = HttpUtility.UrlDecode(token, Encoding.UTF8);
            var result = await _userManager.ResetPasswordAsync(user, token, password);

            return Json(new { succeeded = result.Succeeded, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "重置失败" });
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed) return Json(new { succeeded = true });
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Scheme);
            try
            {
                await _emailSender.SendEmailAsync(email, "重置密码 - SYSU MSC", $"<h2>中山大学微软学生俱乐部</h2><p>您好，请点击 <a href='{callbackUrl}'>此处</a> 重置你的账户密码。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(new { succeeded = true });
        }
    }
}