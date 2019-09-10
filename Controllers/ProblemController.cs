using System.Threading.Tasks;
using FreshBoard.Data;
using FreshBoard.Middlewares;
using FreshBoard.Services;
using FreshBoard.Views.Puzzle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBoard.Controllers
{
    [AdminFilter]
    public class ProblemController : Controller
    {
        private readonly IPuzzleService _puzzleService;
        public ProblemController(IPuzzleService puzzleService)
        {
            _puzzleService = puzzleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var problems = await _puzzleService.QueryProblem().ToListAsync();
            return View(problems);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var problem = await _puzzleService.GetProblemByIdAsync(id);
            return View(problem ?? new Problem { Id = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Problem problem)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (problem.Id == 0)
                await _puzzleService.CreateProblemAsync(problem);
            else
                await _puzzleService.UpdateProblemAsync(problem);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _puzzleService.RemoveProblemAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            var problem = await _puzzleService.GetProblemByIdAsync(id);
            if (problem == null) return View(new ProblemModel
            {
                MessageType = MessageType.Error,
                Message = "找不到这个题目",
                ShowContent = false
            });
            return View(new ProblemModel
            {
                Id = problem.Id,
                Content = problem.Content,
                Script = problem.Script,
                Title = problem.Title,
                Level = problem.Level,
                ShowContent = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> Preview(ProblemModel model)
        {
            var problem = await _puzzleService.GetProblemByIdAsync(model.Id);
            if (problem == null) return View(new ProblemModel
            {
                MessageType = MessageType.Error,
                Message = "找不到这个题目",
                ShowContent = false
            });

            if (problem.Answer == model.Answer)
            {
                model.Message = "答案正确";
                model.MessageType = MessageType.Success;
                model.ShowContent = true;
                ModelState.Clear();
                return View(model);
            }

            model.Message = "答案不正确哦，再仔细想想吧";
            model.MessageType = MessageType.Warning;
            model.ShowContent = true;
            return View(model);
        }
    }
}