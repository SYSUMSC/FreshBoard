using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FreshBoard.Data.Identity;
using FreshBoard.Services;
using System.Threading.Tasks;
using FreshBoard.Middlewares;
using Microsoft.EntityFrameworkCore;
using FreshBoard.Views.Admin;
using System.Linq;
using FreshBoard.Data;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace FreshBoard.Controllers
{
    [AdminFilter]
    public class AdminController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly FreshBoardDbContext _dbContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            FreshBoardDbContext dbContext,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UsersAsync()
        {
            var model = new UsersModel();
            model.Users = await _userManager.Users
                .Select(u => new UsersModel.UserItem()
                {
                    Id = u.Id,
                    Email = u.Email,
                    Phone = u.PhoneNumber,
                    Period =
                        u.Application.IsSuccessful == null ?
                        (u.Application.Period.Title ??
                        _dbContext.ApplicationPeriod.OrderBy(p => p.Order).FirstOrDefault().Title) :
                        (u.Application.IsSuccessful == true ? "成功" : "失败"),
                    HasPrivilege = u.HasPrivilege,
                    PuzzleProgress = u.PuzzleProgress,
                    PersonDataInvalid = _dbContext.UserDataType
                        .GroupJoin(_dbContext.UserData,
                            t => new { t.Id, UserId = u.Id },
                            v => new { Id = v.DataTypeId, UserId = v.UserId ?? "" },
                            (dataType, dataValues) => new { DataType = dataType, DataValues = dataValues })
                        .SelectMany(r => r.DataValues.DefaultIfEmpty(),
                            (r, dataValue) => dataValue.Value)
                        .Count(r => string.IsNullOrEmpty(r)) > 0,
                    FirstRecord = u.Application.Datas.Any(i => i.DataTypeId == 5 && !string.IsNullOrEmpty(i.Value)),
                    SecondRecord = u.Application.Datas.Any(i => i.DataTypeId == 7 && !string.IsNullOrEmpty(i.Value)),
                })
                .ToListAsync();
            model.PossiblePeriods = await _dbContext.ApplicationPeriod
                .OrderBy(p => p.Order)
                .Select(period => new UsersModel.PeriodItem
                {
                    Name = period.Title,
                    Id = period.Id
                })
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> UserAsync(string id)
        {
            var personalData = await _dbContext.UserDataType
                .GroupJoin(_dbContext.UserData,
                    t => new { t.Id, UserId = id },
                    v => new { Id = v.DataTypeId, UserId = v.UserId ?? "" },
                    (dataType, dataValues) => new { DataType = dataType, DataValues = dataValues })
                .SelectMany(r => r.DataValues.DefaultIfEmpty(),
                    (r, dataValue) => new UserModel.PersonalDataRow
                    {
                        DataTypeId = r.DataType.Id,
                        Title = r.DataType.Title,
                        Description = r.DataType.Description,
                        Value = dataValue.Value ?? string.Empty
                    })
                .ToArrayAsync();
            var user = await _userManager.FindByIdAsync(id);

            var periods = (await _dbContext.ApplicationPeriod
                .OrderBy(p => p.Order)
                .GroupJoin(_dbContext.ApplicationPeriodDataType,
                    o => new { o.Id },
                    i => new { Id = i.PeriodId },
                    (period, dataTypes) => new { Period = period, DataTypes = dataTypes })
                .SelectMany(r => r.DataTypes.DefaultIfEmpty(),
                    (v, dataType) => new { v.Period, DataType = dataType })
                .GroupJoin(_dbContext.ApplicationPeriodData,
                    o => new { o.DataType.Id, UserId = user.Id },
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
                        Data = new UserModel.PeriodData
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
                .Select((group, id) => new UserModel.Period
                {
                    Id = group.FirstOrDefault().Period.Id,
                    Title = group.FirstOrDefault().Period.Title,
                    Summary = group.FirstOrDefault().Period.Summary,
                    Description = group.FirstOrDefault().Period.Description,
                    UserApproved = group.FirstOrDefault().Period.UserApproved,
                    Datas = group.Select(r => r.Data).Where(r => r.Title != null).OrderBy(r => r.Id)
                });
            return View(new UserModel()
            {
                Id = id,
                PersonalData = personalData,
                Periods = periods,
                CurrentPeriod = user.Application?.PeriodId ?? 1,
                ApplicationIsSuccessful = user.Application?.IsSuccessful
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserApplyAsync(string id, Dictionary<int, string> data)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                // 确保用户存在有效申请
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

        [HttpPost]
        public async Task<IActionResult> UpdateUserApplyPeriodAsync(string id, int period, bool? status, bool sendNotification = false)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                // 确保用户存在有效申请
                if (user.Application == null)
                {
                    user.Application = new Application
                    {
                        User = user,
                        PeriodId = 1,
                        IsSuccessful = null
                    };
                }
                // 处理阶段更新
                var prevPeriod = user.Application.Period?.Title ?? "申请";
                user.Application.Period = await _dbContext.ApplicationPeriod.FindAsync(period);
                user.Application.IsSuccessful = status;
                // 发送通知
                if (sendNotification == true)
                {
                    // 发送邮件通知
                    if (user.EmailConfirmed)
                    {
                        if (status != null)
                        {
                            if (status == false)
                                await _emailSender.SendStatusChangeAsync(user.PhoneNumber, false, user.Application.Period?.Title ?? "申请");
                            else if (status == true)
                                await _emailSender.SendStatusChangeAsync(user.PhoneNumber, "全部面试通过");
                        }
                        else
                        {
                            await _emailSender.SendStatusChangeAsync(user.PhoneNumber, prevPeriod, user.Application.Period?.Title ?? "申请");
                        }
                    }

                    // 发送短信通知
                    if (user.PhoneNumberConfirmed)
                    {
                        if (status != null)
                        {
                            if (status == false)
                                await _smsSender.SendStatusChangeAsync(user.PhoneNumber, false, user.Application.Period?.Title ?? "申请");
                            else if (status == true)
                                await _smsSender.SendStatusChangeAsync(user.PhoneNumber, "全部面试通过");
                        }
                        else
                        {
                            await _smsSender.SendStatusChangeAsync(user.PhoneNumber, prevPeriod, user.Application.Period?.Title ?? "申请");
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
                return Json(new { succeeded = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUserApplyPeriodAsync");
                return Json(new { succeeded = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BatchToggleAdminAsync(IEnumerable<string> ids)
        {
            IEnumerable<FreshBoardUser> users = await Task.WhenAll(ids.Select(id => _dbContext.Users.FindAsync(id).AsTask()));
            foreach (var user in users)
            {
                user.HasPrivilege = !user.HasPrivilege;
            }
            await _dbContext.SaveChangesAsync();
            return Json(new { succeeded = true });
        }

        [HttpPost]
        public async Task<IActionResult> BatchUpdateStateAsync(IEnumerable<string> ids,
            int period,
            bool? status,
            bool sendNotification = false)
        {
            IEnumerable<FreshBoardUser> users = await Task.WhenAll(ids.Select(id => _dbContext.Users.FindAsync(id).AsTask()));
            IEnumerable<Task> sendTasks = new HashSet<Task>();
            foreach (var user in users)
            {
                // 确保用户存在有效申请
                if (user.Application == null)
                {
                    user.Application = new Application
                    {
                        User = user,
                        PeriodId = 1,
                        IsSuccessful = null
                    };
                }
                // 处理阶段更新
                var prevPeriod = user.Application.Period?.Title ?? "申请";
                user.Application.Period = await _dbContext.ApplicationPeriod.FindAsync(period);
                user.Application.IsSuccessful = status;
                // 发送通知
                if (sendNotification == true)
                {
                    // 发送邮件通知
                    if (user.EmailConfirmed)
                    {
                        if (status != null)
                        {
                            if (status == false)
                                sendTasks.Append(_emailSender.SendStatusChangeAsync(
                                    user.PhoneNumber,
                                    false,
                                    user.Application.Period?.Title ?? "申请"));
                            else if (status == true)
                                sendTasks.Append(_emailSender.SendStatusChangeAsync(
                                    user.PhoneNumber,
                                    "全部面试通过"));
                        }
                        else
                        {
                            sendTasks.Append(_emailSender.SendStatusChangeAsync(
                                user.PhoneNumber,
                                prevPeriod,
                                user.Application.Period?.Title ?? "申请"));
                        }
                    }

                    // 发送短信通知
                    if (user.PhoneNumberConfirmed)
                    {
                        if (status != null)
                        {
                            if (status == false)
                                sendTasks.Append(_smsSender.SendStatusChangeAsync(
                                    user.PhoneNumber,
                                    false,
                                    user.Application.Period?.Title ?? "申请"));
                            else if (status == true)
                                sendTasks.Append(_smsSender.SendStatusChangeAsync(
                                    user.PhoneNumber,
                                    "全部面试通过"));
                        }
                        else
                        {
                            sendTasks.Append(_smsSender.SendStatusChangeAsync(
                                user.PhoneNumber,
                                prevPeriod,
                                user.Application.Period?.Title ?? "申请"));
                        }
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
            try
            {
                await Task.WhenAll(sendTasks);
                return Json(new { succeeded = true });
            }
            catch
            {
                return Json(new { succeeded = true, message = "通知发送失败" });
            }
        }
    }
}
