using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
            path = HttpUtility.UrlDecode(path, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(path))
            {
                path = string.Empty;
            }

            while (path.StartsWith("/") || path.StartsWith("\\"))
            {
                path = path.Substring(1);
            }

            if (path.Contains("../") || path.Contains("..\\") || path.Contains("/..") || path.Contains("\\.."))
            {
                path = "";
            }

            if (Program.GitRepos.ContainsKey("Blogs"))
            {
                var dir = Path.Combine(Program.GitRepos["Blogs"], path);
                if (!Directory.Exists(dir))
                {
                    return Json(null);
                }

                var fileList = Directory.GetFiles(dir, "*.md")
                    .Where(i => Path.GetFileName(i).ToLower() != "readme.md")
                    .Select(i =>
                        new BlogsRepo
                        {
                            FileName = Path.GetFileName(i),
                            Type = 1,
                            Time = new FileInfo(i).LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss")
                        })
                    .Concat(
                        Directory.GetDirectories(dir)
                        .Where(i => !Path.GetFileName(i).StartsWith("."))
                        .Select(i =>
                            new BlogsRepo
                            {
                                FileName = Path.GetFileName(i),
                                Type = 0,
                                Time = new DirectoryInfo(i).LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss")
                            })
                    ).ToList();
                return Json(new { CurrentPath = path, FileList = fileList });
            }
            else
            {
                return Json(null);
            }
        }

        public async Task<IActionResult> GetBlogContentAsync(string path)
        {
            if (path == null)
            {
                path = string.Empty;
            }

            path = HttpUtility.UrlDecode(path, Encoding.UTF8);

            while (path.StartsWith("/") || path.StartsWith("\\"))
            {
                path = path.Substring(1);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                return Json(null);
            }

            if (Path.GetExtension(path).ToLower() != ".md")
            {
                return Json(null);
            }

            if (Program.GitRepos.ContainsKey("Blogs"))
            {
                var file = Path.Combine(Program.GitRepos["Blogs"], path);
                if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
                {
                    return Json(null);
                }

                try
                {
                    string fileInfo = string.Empty;
                    using (var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "git",
                            Arguments = $"blame -p \"{path}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardInput = true,
                            UseShellExecute = false,
                            WorkingDirectory = Program.GitRepos["Blogs"]
                        }
                    })
                    {
                        process.Start();
                        process.StandardInput.Dispose();
                        fileInfo = await process.StandardOutput.ReadToEndAsync();
                        process.WaitForExit();
                    }

                    string author = string.Empty;
                    DateTime? date = null;
                    string subject = string.Empty;
                    foreach (var i in fileInfo.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (i.StartsWith("author ")) author = i.Substring(7);
                        if (i.StartsWith("author-mail ")) author += " " + i.Substring(12);
                        if (i.StartsWith("author-time ")) date = DateTime.UnixEpoch + TimeSpan.FromSeconds(int.Parse(i.Substring(12)));
                        if (i.StartsWith("summary ")) subject = i.Substring(8);
                        if (i.StartsWith("boundary")) break;
                    }
                    if (date == null) date = new FileInfo(file).LastWriteTime;

                    var content = await System.IO.File.ReadAllTextAsync(file, Encoding.UTF8);
                    return Json(new { path, name = Path.GetFileName(file), time = date?.ToString("yyyy/MM/dd HH:mm:ss"), content, author, subject });
                }
                catch
                {
                    return Json(null);
                }
            }
            else
            {
                return Json(null);
            }
        }
    }
}