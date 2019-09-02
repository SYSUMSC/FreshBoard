using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mscfreshman.Data.Identity;
using mscfreshman.Models;
using mscfreshman.Services;
using mscfreshman.Views.Apply;

namespace mscfreshman.Controllers
{
    public class ApplyController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly Data.DbContext _dbContext;

        public ApplyController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            Data.DbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var periods = (await _dbContext.ApplicationPeriod
                .OrderBy(p => p.Id)
                .GroupJoin(_dbContext.ApplicationPeriodDataType,
                    o => new { o.Id, UserVisible = true },
                    i => new { Id = i.PeriodId, UserVisible = i.UserVisible ?? false },
                    (Period, DataTypes) => new { Period, DataTypes })
                .SelectMany(r => r.DataTypes.DefaultIfEmpty(),
                    (v, DataType) => new { v.Period, DataType })
                .GroupJoin(_dbContext.ApplicationPeriodData,
                    o => new { Id = o.DataType.Id, UserId = _userManager.GetUserId(User) },
                    i => new { Id = i.DataTypeId, UserId = i.ApplicationId },
                    (r, DataValues) => new
                    {
                        r.Period,
                        r.DataType,
                        DataValues = DataValues
                    })
                .SelectMany(r => r.DataValues.DefaultIfEmpty(),
                    (r, DataValue) => new
                    {
                        r.Period,
                        Data = new IndexModel.PeriodData
                        {
                            Id = r.DataType.Id,
                            Title = r.DataType.Title,
                            Description = r.DataType.Description,
                            Editable = r.DataType.UserEditable ?? false,
                            Value = DataValue.Value,
                        },
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
                    Datas = group.Select(r => r.Data).Where(r => r.Title != null),
                });
            return View(new IndexModel
            {
                Periods = periods,
                CurrentPeriod = user.Application?.PeriodId ?? 1,
                ApplicationIsSuccessful = user.Application?.IsSuccessful
            });
        }
    }
}
