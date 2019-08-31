using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using mscfreshman.Models;
using mscfreshman.Services;
using mscfreshman.Views.User;

namespace mscfreshman.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly Data.DbContext _dbContext;

        public UserController(
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

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
#warning "Sub-select used, try to use JOIN instead in EF Core 3.0"
            var personalData = await _dbContext.UserDataType
                .OrderBy(t => t.Id)
                .Select(t => new IndexModel.PersonalDataRow
                {
                    DataTypeId = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Value = t.UserData.Where(d => d.UserId == _userManager.GetUserId(User)).FirstOrDefault().Value
                })
                .ToArrayAsync();
            // .GroupJoin(
            //     _dbContext.UserData.Where(d => d.UserId == _userManager.GetUserId(User)),
            //     d => d.Id,
            //     v => v.DataType.Id,
            //     (d, v) => new IndexModel.PersonalDataRow
            //     {
            //         DataTypeId = d.Id,
            //         Title = d.Title,
            //         Description = d.Description,
            //         Value = v.FirstOrDefault().Value
            //     })
            // .ToArrayAsync();
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
                        existingData[kv.Key].Value = kv.Value;
                    }
                    else
                    {
                        newData.Add(new UserData
                        {
                            UserId = userId,
                            DataTypeId = kv.Key,
                            Value = kv.Value
                        });
                    }
                }
                _dbContext.AddRange(newData);
                await _dbContext.SaveChangesAsync();
                return Json(new { succeeded = true });
            }
            catch (Exception ex)
            {
                return Json(new { succeeded = false, message = ex.Message });
            }
        }
    }
}
