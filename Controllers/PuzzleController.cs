using System;
using System.Threading.Tasks;
using FreshBoard.Data.Identity;
using FreshBoard.Extensions;
using FreshBoard.Services;
using FreshBoard.Views.Puzzle;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreshBoard.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class PuzzleController : Controller
    {
        private readonly IPuzzleService _puzzleService;
        private readonly UserManager<FreshBoardUser> _userManager;

        public PuzzleController(IPuzzleService puzzleService, UserManager<FreshBoardUser> userManager)
        {
            _puzzleService = puzzleService;
            _userManager = userManager;
        }

        private async Task<(ProblemModel Model, bool Succeeded)> GetProblem(FreshBoardUser user)
        {
            if (user == null)
            {
                return (new ProblemModel
                {
                    MessageType = MessageType.Error,
                    Message = "没有登录账户",
                    ShowContent = false
                }, false);
            }
            var problems = await _puzzleService.GetProblemsByLevelAsync(user.PuzzleProgress + 1).ToListAsync();
            if (problems.Count == 0) return (new ProblemModel
            {
                MessageType = MessageType.Success,
                Message = "你完成了全部的题目",
                ShowContent = false
            }, false);

            var index = (int)(Math.Abs(BitConverter.ToInt64(Guid.Parse(user.Id).ToByteArray())) % problems.Count);
            var problem = problems[index];
            return (new ProblemModel
            {
                Id = problem.Id,
                Content = problem.Content,
                Script = problem.Script,
                Title = problem.Title,
                Level = problem.Level,
                ShowContent = true
            }, true);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var (model, _) = await GetProblem(user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProblemModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return View(new ProblemModel
            {
                MessageType = MessageType.Error,
                Message = "没有登录账户",
                ShowContent = false
            });
            var problem = await _puzzleService.GetProblemByIdAsync(model.Id);

            if (problem == null)
            {
                await _puzzleService.RecordSubmission(model.Id, user.Id, model.Answer, 3);
                return View(new ProblemModel
                {
                    MessageType = MessageType.Error,
                    Message = "找不到这个题目",
                    ShowContent = false
                });
            }

            if (problem.Level > user.PuzzleProgress + 1)
            {
                await _puzzleService.RecordSubmission(problem.Id, user.Id, model.Answer, 2);
                return View(new ProblemModel
                {
                    MessageType = MessageType.Error,
                    Message = "没有这种操作",
                    ShowContent = false
                });
            }

            if (problem.Answer == model.Answer)
            {
                user.PuzzleProgress++;
                await _userManager.UpdateAsync(user);
                await _puzzleService.RecordSubmission(problem.Id, user.Id, model.Answer, 1);
                var (newModel, succeeded) = await GetProblem(user);
                if (succeeded)
                {
                    newModel.MessageType = MessageType.Success;
                    newModel.Message = "答案正确，进入下一题";
                    newModel.ShowContent = true;
                }
                ModelState.Clear();
                return View(newModel);
            }

            await _puzzleService.RecordSubmission(problem.Id, user.Id, model.Answer, 0);
            (model, _) = await GetProblem(user);
            model.Message = "答案不正确哦，再仔细想想吧";
            model.MessageType = MessageType.Warning;
            model.ShowContent = true;
            return View(model);
        }
    }
}
