using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FreshBoard.Data;
using FreshBoard.Middlewares;
using FreshBoard.Services;
using FreshBoard.Views.Problem;
using FreshBoard.Views.Puzzle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBoard.Controllers
{
    [AdminFilter]
    public class ProblemController : Controller
    {
        private readonly IPuzzleService _puzzleService;
        private readonly FreshBoardDbContext _dbContext;
        public ProblemController(IPuzzleService puzzleService, FreshBoardDbContext dbContext)
        {
            _puzzleService = puzzleService;
            _dbContext = dbContext;
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
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var midPath = Guid.NewGuid().ToString().Replace("-", "");
            if (!Directory.Exists($"wwwroot/upload/{midPath}")) Directory.CreateDirectory($"wwwroot/upload/{midPath}");
            var path = Path.Combine(midPath, file.FileName);
            await using var fileStream = new FileStream($"wwwroot/upload/{path}", FileMode.Create);
            await file.CopyToAsync(fileStream);
            return Content($"/upload/{midPath}/{file.FileName}");
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

            model.Content = problem.Content;
            model.Level = problem.Level;
            model.Script = problem.Script;
            model.Title = problem.Title;

            if (problem.Answer == model.Answer)
            {
                model.Message = "答案正确";
                model.MessageType = MessageType.Success;
                model.ShowContent = true;
                model.Answer = "";
                ModelState.Clear();
                return View(model);
            }

            model.Message = "答案不正确";
            model.MessageType = MessageType.Warning;
            model.ShowContent = true;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Ground(int page = 1)
        {
            ViewBag.CurrentPage = page;
            var records = from c in _dbContext.PuzzleRecord
                          where c.Id > (page - 1) * 50
                          let x = _dbContext.Problem.First(i => i.Id == c.ProblemId)
                          let y = _dbContext.Users.First(i => i.Id == c.UserId)
                          orderby c.Id
                          select new GroundModel
                          {
                              Id = c.Id,
                              Level = x.Level,
                              ProblemId = x.Id,
                              ProblemName = x.Title,
                              UserId = y.Id,
                              UserName = y.UserName,
                              Answer = c.Content
                          };
            return View(await records.Take(50).ToListAsync());
        }
    }
}