using FreshBoard.Data;
using FreshBoard.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FreshBoard.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly DbContextOptions<FreshBoardDbContext> _dbContextOptions;
        public ChatHub(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            DbContextOptions<FreshBoardDbContext> dbContextOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContextOptions = dbContextOptions;
        }
    }
}
