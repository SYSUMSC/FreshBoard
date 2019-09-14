using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreshBoard.Data;
using FreshBoard.Data.Identity;
using FreshBoard.Services;
using FreshBoard.Views.Apply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FreshBoard.Controllers
{
    [Authorize]
    public class ApplyController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly FreshBoardDbContext _dbContext;
        private readonly ILogger<ApplyController> _logger;

        public ApplyController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            FreshBoardDbContext dbContext,
            ILogger<ApplyController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var application = await _dbContext.Application.FindAsync(user.Id);
            var periods = (await _dbContext.ApplicationPeriod
                .OrderBy(p => p.Order)
                .GroupJoin(_dbContext.ApplicationPeriodDataType,
                    o => new { o.Id, UserVisible = true },
                    i => new { Id = i.PeriodId, UserVisible = i.UserVisible ?? false },
                    (period, dataTypes) => new { Period = period, DataTypes = dataTypes })
                .SelectMany(r => r.DataTypes.DefaultIfEmpty(),
                    (v, dataType) => new { v.Period, DataType = dataType })
                .GroupJoin(_dbContext.ApplicationPeriodData,
                    o => new { o.DataType.Id, UserId = _userManager.GetUserId(User) },
                    i => new { Id = i.DataTypeId, UserId = i.ApplicationId ?? "" },
                    (r, dataValues) => new
                    {
                        r.Period,
                        r.DataType,
                        DataValues = dataValues
                    })
                .SelectMany(r => r.DataValues.DefaultIfEmpty(),
                    (r, dataValue) => new
                    {
                        r.Period,
                        Data = new IndexModel.PeriodData
                        {
                            Id = r.DataType.Id,
                            Title = r.DataType.Title,
                            Description = r.DataType.Description,
                            Editable = r.DataType.UserEditable ?? false,
                            Value = dataValue.Value
                        }
                    })
                .ToArrayAsync())
                .GroupBy(r => r.Period.Id)
                .Select((group, id) => new IndexModel.Period
                {
                    Id = group.FirstOrDefault().Period.Id,
                    Title = group.FirstOrDefault().Period.Title,
                    Summary = group.FirstOrDefault().Period.Summary,
                    Description = group.FirstOrDefault().Period.Description,
                    UserApproved = group.FirstOrDefault().Period.UserApproved,
                    Datas = group.Select(r => r.Data).Where(r => r.Title != null).OrderBy(r => r.Id)
                });
            return View(new IndexModel
            {
                Periods = periods,
                CurrentPeriod = user.Application?.PeriodId ?? 1,
                ApplicationIsSuccessful = user.Application?.IsSuccessful,
                PersonDataInvalid = (await _dbContext.UserDataType
                .GroupJoin(_dbContext.UserData,
                    t => new { t.Id, UserId = _userManager.GetUserId(User) },
                    v => new { Id = v.DataTypeId, UserId = v.UserId ?? "" },
                    (dataType, dataValues) => new { DataType = dataType, DataValues = dataValues })
                .SelectMany(r => r.DataValues.DefaultIfEmpty(),
                    (r, dataValue) => dataValue.Value)
                .CountAsync(r => string.IsNullOrEmpty(r))) > 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Dictionary<int, string> data)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                // 本行查询是必要的，以保证加载相关的申请信息
                var application = await _dbContext.Application.FindAsync(user.Id);
                // 确保用户存在有效申请
                // if ((await _dbContext.Application.FindAsync(user.Id)) == null)
                if (user.Application == null)
                {
                    user.Application = new Application
                    {
                        User = user,
                        PeriodId = 1,
                        IsSuccessful = null
                    };
                }
                // 校验申请信息修改合法性
                var dataTypes = await _dbContext.ApplicationPeriodDataType.ToDictionaryAsync(e => e.Id);
                foreach (var key in data.Keys)
                {
                    if (!dataTypes.ContainsKey(key))
                        throw new Exception("指定的申请信息类型无效");
                    if (dataTypes[key].UserEditable == false || dataTypes[key].UserVisible == false)
                        throw new Exception("申请信息不允许被用户修改");
                    if (dataTypes[key].PeriodId != (user.Application?.PeriodId ?? 1))
                        throw new Exception("非当前申请阶段的信息不允许修改");
                }
                // 匹配数据库已有的信息并修改
                var existingData = await _dbContext.ApplicationPeriodData
                    .Where(d => d.ApplicationId == user.Id)
                    .ToDictionaryAsync(d => d.DataTypeId);
                var newData = new List<ApplicationPeriodData>();
                foreach (var (key, value) in data)
                {
                    if (existingData.ContainsKey(key))
                    {
                        existingData[key].Value = value ?? string.Empty;
                    }
                    else
                    {
                        newData.Add(new ApplicationPeriodData
                        {
                            ApplicationId = user.Id,
                            DataTypeId = key,
                            Value = value ?? string.Empty
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
