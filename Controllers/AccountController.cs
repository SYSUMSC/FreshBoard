using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using mscfreshman.Data.Identity;
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

        [HttpGet]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            return Json(new
            {
                isSignedIn = _signInManager.IsSignedIn(User),
                userInfo = await _userManager.GetUserAsync(User)
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
            {
                return Redirect("/");
            }

            return Json(new { code = 1, message = "用户名或密码不正确" });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(string name, string email, int grade, string phone, string qq, string password, string confirmpassword)
        {
            if (password != confirmpassword)
            {
                return Json(new { code = 2, message = "密码和确认密码不匹配" });
            }
            var user = new FreshBoardUser
            {
                UserName = email,
                Email = email,
                Name = name,
                Grade = grade,
                PhoneNumber = phone,
                QQ = qq
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new { userId = user.Id, code }, Request.Scheme);

                await _emailSender.SendEmailAsync(email, "验证邮箱", $"请点击 <a href='{callbackUrl}'></a> 验证你的邮箱地址");

                var signInResult = await _signInManager.PasswordSignInAsync(email, password, false, false);
                if (signInResult.Succeeded)
                {
                    return Redirect("/");
                }
            }

            return Json(new { code = 3, message = "注册失败", errors = result.Errors });
        }
    }
}