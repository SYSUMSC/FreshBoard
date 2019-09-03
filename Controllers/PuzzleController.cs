using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreshBoard.Data.Identity;
using FreshBoard.Services;
using FreshBoard.Views.Puzzle;
using Microsoft.AspNetCore.Identity;

namespace FreshBoard.Controllers
{
    public class PuzzleController : Controller
    {
        private readonly IPuzzleService _puzzleService;
        private readonly UserManager<FreshBoardUser> _userManager;

        public PuzzleController(IPuzzleService puzzleService, UserManager<FreshBoardUser> userManager)
        {
            _puzzleService = puzzleService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            // TODO: show an error message in View
            if (user == null) throw new Exception("用户未登录");
            var problems = _puzzleService.GetProblemsByLevelAsync(user.PuzzleProgress);
            await foreach (var i in problems)
            {
                return View(new ProblemModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    Script = i.Script,
                    Content = i.Content
                });
            }

            // TODO: show an error message in View
            throw new NotImplementedException("没这个难度的题");
            // return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, string answer)
        {
            var user = await _userManager.GetUserAsync(User);
            // TODO: show an error message in View
            if (user == null) throw new Exception("用户未登录");
            var problems = _puzzleService.GetProblemsByLevelAsync(user.PuzzleProgress);
            await foreach (var i in problems)
            {
                // TODO: UI
                if (i.Answer == answer) return Ok();
                return Forbid();
            }
            return Forbid();
        }
    }
}
