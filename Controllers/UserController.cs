using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using FreshBoard.Models;

namespace FreshBoard.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly Data.DbContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            Data.DbContext dbContext,
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
                    (DataType, DataValues) => new { DataType, DataValues })
                .SelectMany(r => r.DataValues.DefaultIfEmpty(),
                    (r, DataValue) => new IndexModel.PersonalDataRow
                    {
                        DataTypeId = r.DataType.Id,
                        Title = r.DataType.Title,
                        Description = r.DataType.Description,
                        Value = DataValue.Value ?? string.Empty
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
                List<UserData> newData = new List<UserData>();
                foreach (var kv in data)
                {
                    if (existingData.ContainsKey(kv.Key))
                    {
                        existingData[kv.Key].Value = kv.Value ?? String.Empty;
                    }
                    else
                    {
                        newData.Add(new UserData
                        {
                            UserId = userId,
                            DataTypeId = kv.Key,
                            Value = kv.Value ?? String.Empty
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
    }
}
