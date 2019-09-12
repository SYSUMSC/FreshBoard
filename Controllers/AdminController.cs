using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FreshBoard.Data.Identity;
using FreshBoard.Services;
using System.Threading.Tasks;
using FreshBoard.Middlewares;
using Microsoft.EntityFrameworkCore;

namespace FreshBoard.Controllers
{
    [AdminFilter]
    public class AdminController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public AdminController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UsersAsync()
        {
            _userManager.Users.ToListAsync();
            return View();
        }
    }
}