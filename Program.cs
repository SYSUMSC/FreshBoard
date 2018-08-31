using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace mscfreshman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Thread(
                () => GitWorkingThread(
                    "https://github.com/SYSU-MSC-Studio/Blogs.git",
                     Path.Combine(Environment.CurrentDirectory, "BlogsRepo"), 
                     "Blogs"))
                .Start();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static readonly ConcurrentDictionary<string, string> GitRepos = new ConcurrentDictionary<string, string>();
        public static void GitWorkingThread(string gitUrl, string saveDir, string name)
        {
            GitRepos[name] = saveDir;
            if (!Directory.Exists(saveDir))
            {
                Process.Start("git", $"clone {gitUrl} {saveDir}").WaitForExit();
            }
            while (!Environment.HasShutdownStarted)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "pull --force",
                    WorkingDirectory = saveDir,
                    UseShellExecute = false
                }).WaitForExit();
                Thread.Sleep(300 * 1000);
            }
        }
    }
}
