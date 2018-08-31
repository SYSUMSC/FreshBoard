using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using mscfreshman.Data.Identity;
using System;
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

        [HttpGet]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            var data = new
            {
                isSignedIn = _signInManager.IsSignedIn(User),
                userInfo = await _userManager.GetUserAsync(User)
            };
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string email, string password, bool persistent = true)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, persistent, false);
            if (result.Succeeded)
            {
                return Redirect("/");
            }

            return Json(new { code = 1, message = "用户名或密码不正确" });
        }

        [HttpPost]
        public async Task<IActionResult> SendRegisterEmailAsync()
        {
            if (!_signInManager.IsSignedIn(User)) return Json(new { succeeded = false, message = "未登录" });
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { succeeded = false, message = "发生未知错误" });

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var email = user.Email;
            var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new { userId = user.Id, code }, Request.Scheme);

            await _emailSender.SendEmailAsync(email, "验证邮箱", $"<h2>中山大学微软学生俱乐部</h2><p>感谢您的注册，请点击 <a href='{callbackUrl}'></a> 验证你的邮箱地址。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");

            return Json(new { succeeded = true });

        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(
            string name, //姓名
            string email, //email
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
            if (password != confirmpassword)
            {
                return Json(new { succeeded = false, message = "密码和确认密码不匹配" });
            }
            var user = new FreshBoardUser
            {
                UserName = email,
                Email = email,
                Name = name,
                Grade = grade,
                PhoneNumber = phone,
                WeChat = wechat,
                QQ = qq,
                Sexual = sexual,
                CPCLevel = cpclevel,
                Institute = institute,
                Major = major,
                SchoolNumber = schnum
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new { userId = user.Id, code }, Request.Scheme);

                await _emailSender.SendEmailAsync(email, "验证邮箱", $"<h2>中山大学微软学生俱乐部</h2><p>感谢您的注册，请点击 <a href='{callbackUrl}'></a> 验证你的邮箱地址。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");

                var signInResult = await _signInManager.PasswordSignInAsync(email, password, true, false);
                return Redirect("/");
            }

            return Json(new { succeeded = false, message = "注册失败", errors = result.Errors });
        }
    }
}