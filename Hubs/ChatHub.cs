using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mscfreshman.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        public ChatHub(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContextOptions = dbContextOptions;
        }
    }
}
