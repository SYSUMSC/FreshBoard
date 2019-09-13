using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FreshBoard.Data;
using FreshBoard.Data.Identity;
using FreshBoard.Services;
using FreshBoard.Views.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FreshBoard.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly FreshBoardDbContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            FreshBoardDbContext dbContext,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var personalData = await _dbContext.UserDataType
                .GroupJoin(_dbContext.UserData,
                    t => new { t.Id, UserId = _userManager.GetUserId(User) },
                    v => new { Id = v.DataTypeId, UserId = v.UserId ?? "" },
                    (dataType, dataValues) => new { DataType = dataType, DataValues = dataValues })
                .SelectMany(r => r.DataValues.DefaultIfEmpty(),
                    (r, dataValue) => new IndexModel.PersonalDataRow
                    {
                        DataTypeId = r.DataType.Id,
                        Title = r.DataType.Title,
                        Description = r.DataType.Description,
                        Value = dataValue.Value ?? string.Empty
                    })
                .ToArrayAsync();
            return View(new IndexModel { PersonalData = personalData });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Dictionary<int, string> data)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var existingData = await _dbContext.UserData
                    .Where(d => d.UserId == userId)
                    .ToDictionaryAsync(d => d.DataTypeId);
                var newData = new List<UserData>();
                foreach (var kv in data)
                {
                    if (existingData.ContainsKey(kv.Key))
                    {
                        existingData[kv.Key].Value = kv.Value ?? string.Empty;
                    }
                    else
                    {
                        newData.Add(new UserData
                        {
                            UserId = userId,
                            DataTypeId = kv.Key,
                            Value = kv.Value ?? string.Empty
                        });
                    }
                }
                _dbContext.AddRange(newData);
                await _dbContext.SaveChangesAsync();
                return Json(new { succeeded = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新用户信息时发生错误");
                return Json(new { succeeded = false, message = ex.Message });
            }
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
            var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, code }, Request.Scheme);

            try
            {
                await _emailSender.SendEmailAsync(email, "验证邮箱 - SYSU MSC", $"<h2>中山大学微软学生俱乐部</h2><p>感谢您的注册，请点击 <a href='{callbackUrl}'>此处</a> 验证你的邮箱地址。</p><hr /><p>请勿回复本邮件</p><p>{DateTime.Now} - SYSU MSC</p>");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while sending confirmation email.");
                return Json(new { succeeded = false, message = "邮件发送失败" });
            }

            return Json(new { succeeded = true });

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("找不到该用户");
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded) throw new Exception(String.Join("; ", result.Errors.Select(e => e.Description)));
            return Redirect(Url.Action("Index"));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCredentialAsync(string email, string phone)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!string.IsNullOrEmpty(email) && user.Email != email)
            {
                await _userManager.SetEmailAsync(user, email);
                user.UserName = email;
                await _signInManager.RefreshSignInAsync(user);
            }
            if (!string.IsNullOrEmpty(phone) && user.PhoneNumber != phone)
                await _userManager.SetPhoneNumberAsync(user, phone);
            await _userManager.UpdateAsync(user);
            return Json(new { succeeded = true });
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
                await _smsSender.SendValidationCodeAsync(user.PhoneNumber, token);
                return Json(new { succeeded = true, message = "短信发送成功" });
            }
            catch (SmsException ex)
            {
                return Json(new { succeeded = false, message = ex.Message });
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
    }
}
