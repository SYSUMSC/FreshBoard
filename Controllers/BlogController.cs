using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mscfreshman.Controllers
{
    public class BlogController : Controller
    {
        private readonly UserManager<FreshBoardUser> _userManager;
        private readonly SignInManager<FreshBoardUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        public BlogController(
            UserManager<FreshBoardUser> userManager,
            SignInManager<FreshBoardUser> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _dbContextOptions = dbContextOptions;
        }

        public class BlogsRepo
        {
            public string FileName;
            public int Type; // 0. Directory, 1. File
            public string Time;
        }

        public IActionResult GetBlogTree(string path)
        {
            if (string.IsNullOrEmpty(path)) path = string.Empty;
            while (path.StartsWith("/") || path.StartsWith("\\")) path = path.Substring(1);
            if (path.Contains("../") || path.Contains("..\\") || path.Contains("/..") || path.Contains("\\..")) path = "";
            if (Program.GitRepos.ContainsKey("Blogs"))
            {
                var dir = Path.Combine(Program.GitRepos["Blogs"], path);
                if (!Directory.Exists(dir)) return Json(null);
                var fileList = Directory.GetFiles(dir, "*.md")
                    .Where(i => i.ToLower() != "readme.md")
                    .Select(i =>
                        new BlogsRepo
                        {
                            FileName = Path.GetFileName(i),
                            Type = 1,
                            Time = new FileInfo(i).LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss")
                        })
                    .Concat(
                        Directory.GetDirectories(dir).Select(i =>
                            new BlogsRepo
                            {
                                FileName = Path.GetFileName(i),
                                Type = 0,
                                Time = new DirectoryInfo(i).LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss")
                            })
                    );
                return Json(new { CurrentPath = path, FileList = fileList });
            }
            else return Json(null);
        }

        public async Task<IActionResult> GetBlogContentAsync(string path)
        {
            if (string.IsNullOrEmpty(path)) path = string.Empty;
            while (path.StartsWith("/") || path.StartsWith("\\")) path = path.Substring(1);
            if (Path.GetExtension(path).ToLower() != ".md") return Json(null);
            if (Program.GitRepos.ContainsKey("Blogs"))
            {
                path = Path.Combine(Program.GitRepos["Blogs"], path);
                if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return Json(null);
                try
                {
                    var content = await System.IO.File.ReadAllTextAsync(path, new UTF8Encoding(false));
                    return Json(new { path = path, name = Path.GetFileName(path), time = new FileInfo(path).LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"), content = content });
                }
                catch
                {
                    return Json(null);
                }
            }
            else return Json(null);
        }
    }
}