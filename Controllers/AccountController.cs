using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mscfreshman.Data;

namespace mscfreshman.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        public AccountController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            return Json(new { email, password });
        }

        [HttpPost]
        public IActionResult Register(string name, string email, int grade, string phone, string qq, string password, string confirmpassword)
        {
            return Json(new { name, email, grade, phone, qq, password, confirmpassword });
        }
    }
}