using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace mscfreshman.Controllers
{
    public class ProblemController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        public ProblemController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _dbContextOptions = dbContextOptions;
        }

        [HttpGet]
        public async Task<IActionResult> GetProblemAsync()
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

            if (user.CrackProgress == 10)
            {
                return Json(new { succeeded = false, message = "您已完成了所有的题目" });
            }

            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                var problems = db.Problem.GroupBy(i => i.Level);

                if (problems.Any(i => i.Key == user.CrackProgress + 1))
                {
                    var set = problems.FirstOrDefault(i => i.Key == user.CrackProgress + 1);
                    var random = new Random();
                    return Json(new
                    {
                        succeeded = true,
                        problem = set.Skip(random.Next(0, set.Count() - 1))
                        .Take(1).Select(i => new
                        {
                            i.Id,
                            i.Level,
                            i.Script,
                            i.Content,
                            i.Title
                        }).FirstOrDefault()
                    });
                }
                else
                {
                    return Json(new { succeeded = false, message = "找不到题目" });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswerAsync(int pid, string answer)
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

            using (var db = new ApplicationDbContext(_dbContextOptions))
            {
                var problem = await db.Problem.FindAsync(pid);
                if (problem == null)
                {
                    return Json(new { succeeded = false, message = "找不到题目" });
                }

                if (problem.Level != user.CrackProgress + 1)
                {
                    return Json(new { succeeded = false, message = "参数错误" });
                }

                if (problem.Answer != answer)
                {
                    return Json(new { succeeded = false, message = "答案不正确" });
                }
                if (user.CrackProgress < 10)
                {
                    user.CrackProgress++;
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    if (user.ApplyStatus == 1)
                    {
                        user.ApplyStatus = 2;
                        await _userManager.UpdateAsync(user);
                    }
                }
                return Json(new { succeeded = true });
            }
        }
    }
}