using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using FreshBoard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreshBoard.Data.Identity;

namespace FreshBoard.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly DbContextOptions<Data.FreshBoardDbContext> _dbContextOptions;
        public ChatHub(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            DbContextOptions<Data.FreshBoardDbContext> dbContextOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContextOptions = dbContextOptions;
        }
    }
}
