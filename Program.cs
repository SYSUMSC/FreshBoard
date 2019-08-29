using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace mscfreshman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* new Thread(
                () => GitWorkingThread(
                    "https://github.com/SYSU-MSC-Studio/Blogs.git",
                     Path.Combine(Environment.CurrentDirectory, "BlogsRepo"), 
                     "Blogs"))
                .Start(); */
            // SmsReceiver.StartThread();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
#if DEBUG
                    // systemfd debug support, only on Unix system
                    // run the following command to get continous
                    // socket listening
                    //   systemfd --no-pid -s http::5000 -- dotnet watch run
                    #region systemfd
                    // enable libuv to support ListenHandle
                    .UseLibuv()
                    .ConfigureKestrel(serverOptions =>
                    {
                        // detect systemfd environment
                        var fds = Environment.GetEnvironmentVariable("LISTEN_FDS");
                        if (fds != null)
                            // listen to given file handle
                            // file handle begins at 3
                            serverOptions.ListenHandle(3);
                    })
                    #endregion
#endif
                        .UseStartup<Startup>();
                });

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
                Thread.Sleep(120 * 1000);
            }
        }
    }
}
