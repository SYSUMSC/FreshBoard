using System.Threading.Tasks;
using FreshBoard.Data;
using FreshBoard.Services;
using FreshBoard.Views.Puzzle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBoard.Controllers
{
    public class ProblemController : Controller
    {
        private readonly IPuzzleService _puzzleService;
        public ProblemController(IPuzzleService puzzleService)
        {
            _puzzleService = puzzleService;
        }

        private bool Authenticate()
        {
            if (!Request.Cookies.TryGetValue("problem", out var value)) return false;
            return value == "vnu3uvnwmerovdg";
        }

        [Route("acsn973ncrw3rv")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Authenticate()) return Forbid();
            var problems = await _puzzleService.QueryProblem().ToListAsync();
            return View(problems);
        }

        [Route("vyw8rn3cu89rmwr")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!Authenticate()) return Forbid();
            var problem = await _puzzleService.GetProblemByIdAsync(id);
            return View(problem ?? new Problem { Id = 0 });
        }

        [Route("vyw8rn3cu89rmwr")]
        [HttpPost]
        public async Task<IActionResult> Edit(Problem problem)
        {
            if (!Authenticate()) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            if (problem.Id == 0)
                await _puzzleService.CreateProblemAsync(problem);
            else
                await _puzzleService.UpdateProblemAsync(problem);
            return RedirectToAction("Index");
        }

        [Route("nuvr98wenrcw3dg")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Authenticate()) return Forbid();
            await _puzzleService.RemoveProblemAsync(id);
            return RedirectToAction("Index");
        }

        [Route("sdf4tvtnwbtevwe")]
        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            if (!Authenticate()) return Forbid();
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

        [Route("sdf4tvtnwbtevwe")]
        [HttpPost]
        public async Task<IActionResult> Preview(ProblemModel model)
        {
            if (!Authenticate()) return Forbid();
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