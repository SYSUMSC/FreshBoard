using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreshBoard.Services;

namespace FreshBoard.Controllers
{
    public class PuzzleController : Controller
    {
        private readonly IPuzzleService _puzzleService;

        public PuzzleController(IPuzzleService puzzleService)
        {
            _puzzleService = puzzleService;
        }

        [HttpGet]
        public IActionResult Index() => View();
    }
}
