using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mscfreshman.Data;

namespace mscfreshman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
#if DEBUG
            if (args.Length >= 1 && args[0] == "--sync-db")
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    try
                    {
                        var context = services.GetService<DbContext>();
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        logger.LogInformation("Database creation succeeded.");

                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "姓名",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "年级",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "QQ",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "微信",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "政治面貌",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "性别",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "学号",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "学院",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "专业",
                            Description = ""
                        });
                        context.UserDataType.Add(new UserDataType
                        {
                            Title = "出生日期",
                            Description = ""
                        });

                        context.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }
#endif
            /* new Thread(
                () => GitWorkingThread(
                    "https://github.com/SYSU-MSC-Studio/Blogs.git",
                     Path.Combine(Environment.CurrentDirectory, "BlogsRepo"), 
                     "Blogs"))
                .Start(); */
            // SmsReceiver.StartThread();
            host.Run();
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
