using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FreshBoard.Data.Identity;
using FreshBoard.Models;
using FreshBoard.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FreshBoard.Controllers
{
    [Route("/{action=Index}/{id?}")]
    public class HomeController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger<HomeController> _logger;
        private readonly Data.FreshBoardDbContext _dbContext;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            Data.FreshBoardDbContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error([FromQuery]int code = 500)
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code,
                Exception = code != 500 ? null : exceptionHandlerPathFeature?.Error
            });
        }

        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost("/SignIn")]
        public async Task<IActionResult> SigninPostAsync(string email, string password, bool persistent = true)
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

            var result = await _signInManager.PasswordSignInAsync(email,
                password,
                persistent,
                await _userManager.Users.CountAsync() <= 0);

            if (result.Succeeded)
            {
                return Json(new { succeeded = true });
            }

            return Json(new { succeeded = false, message = "用户名或密码不正确" });
        }

        [HttpPost("/SignUp")]
        public async Task<IActionResult> RegisterAsync(
            string email, // email
            string password,// 密码
            string phone // 手机号
        )
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { succeeded = false, message = "未填写邮箱" });
            }

            if (string.IsNullOrEmpty(password))
            {
                return Json(new { succeeded = false, message = "未填写密码" });
            }

            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { succeeded = false, message = "未填写手机号" });
            }

            var user = new FreshBoardUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = phone,
                Privilege = (await _userManager.Users.CountAsync()) == 0 ? 1 : 0
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, code }, Request.Scheme);
                try
                {
                    await _emailSender.SendEmailAsync(email, "验证邮箱 - SYSU MSC", $"<h2>中山大学微软学生俱乐部</h2><p>感谢您的注册，请点击 <a href='{callbackUrl}'>此处</a> 验证你的邮箱地址。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while sending confirmation email.");
                    Console.WriteLine(ex.Message);
                }
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, false);
                return Json(new { succeeded = true });
            }

            return Json(new { succeeded = false, message = result.Errors.Any() ? result.Errors.Select(i => i.Description).Aggregate((accu, next) => accu + "\n" + next) : "注册失败" });
        }

        public async Task<IActionResult> SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
