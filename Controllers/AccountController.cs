using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mscfreshman.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public IActionResult Login()
        {
            return Json(null);
        }

        [HttpPost]
        public IActionResult Register()
        {
            return Json(null);
        }
    }
}