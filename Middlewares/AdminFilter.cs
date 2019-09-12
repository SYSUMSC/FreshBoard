using System;
using System.Threading.Tasks;
using FreshBoard.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace FreshBoard.Middlewares
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class AdminFilterAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<FreshBoardUser>>();
            var user = await userManager.GetUserAsync(context.HttpContext.User);
            if ((user?.Privilege ?? 0) == 1) await next();

            context.Result = new RedirectToActionResult("Error", "Home", new { code = 403 });
        }
    }
}
