using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FreshBoard.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FreshBoard
{
    public static class WebBuilderExtensions
    {
        public static IWebHostBuilder UseSystemfdLiveReload(this IWebHostBuilder builder)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) return builder;
            return builder
            #region systemfd
#if DEBUG
                    // systemfd debug support, only on Unix system
                    // run the following command to get continous
                    // socket listening
                    //   systemfd --no-pid -s http::5000 -- dotnet watch run
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
#endif
            #endregion
                    ;
        }
    }
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            #region SyncDatabase
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

                        context.ApplicationPeriod.Add(new ApplicationPeriod(1, "申请", "递交申请，自动通过", userApproved: true));
                        context.ApplicationPeriod.Add(new ApplicationPeriod(2, "一面", "第一次面试"));
                        context.ApplicationPeriod.Add(new ApplicationPeriod(3, "二面", "第二次面试"));

                        context.ApplicationPeriodDataType.AddRange(
                            new[] {
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(1),
                                    "简单的自我介绍一下",
                                    ""),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(1),
                                    "你有什么个性或者特长",
                                    ""),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(1),
                                    "你对俱乐部有什么了解",
                                    ""),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(1),
                                    "为什么想要加入俱乐部",
                                    ""),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(1),
                                    "你认为你能给俱乐部带来什么",
                                    ""),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(1),
                                    "你觉得俱乐部能给你带来什么",
                                    ""),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(1),
                                    "还有什么其他想补充的",
                                    "可以留空"),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(2),
                                    "一面评价",
                                    "", false, false),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(3),
                                    "分组",
                                    "", true, false),
                                new ApplicationPeriodDataType(
                                    context.ApplicationPeriod.Find(3),
                                    "二面评价",
                                    "", false, false),
                            }
                        );

                        context.Problem.AddRange(new[] {
                            new Problem
                            {
                                Level = 1,
                                Title = "测试题目 1.1",
                                Content = "<p>Test<a href=\"javascript:void(0)\" onclick=\"emmm()\">？</a></p>",
                                Script = "function emmm() { alert('123456') }",
                                Answer = "123456"
                            },
                            new Problem
                            {
                                Level = 1,
                                Title = "测试题目 1.2",
                                Content = "<p>Test<a href=\"javascript:void(0)\" onclick=\"emmm()\">？</a></p>",
                                Script = "function emmm() { alert('123456') }",
                                Answer = "123456"
                            },
                            new Problem
                            {
                                Level = 2,
                                Title = "测试题目 2",
                                Content = "<p>Test<a href=\"javascript:void(0)\" onclick=\"emmm()\">？</a></p>",
                                Script = "function emmm() { alert('123456') }",
                                Answer = "123456"
                            },
                            new Problem
                            {
                                Level = 3,
                                Title = "测试题目 3.1",
                                Content = "<p>Test<a href=\"javascript:void(0)\" onclick=\"emmm()\">？</a></p>",
                                Script = "function emmm() { alert('123456') }",
                                Answer = "123456"
                            },
                            new Problem
                            {
                                Level = 3,
                                Title = "测试题目 3.2",
                                Content = "<p>Test<a href=\"javascript:void(0)\" onclick=\"emmm()\">？</a></p>",
                                Script = "function emmm() { alert('123456') }",
                                Answer = "123456"
                            },
                            new Problem
                            {
                                Level = 4,
                                Title = "测试题目 4",
                                Content = "<p>Test<a href=\"javascript:void(0)\" onclick=\"emmm()\">？</a></p>",
                                Script = "function emmm() { alert('123456') }",
                                Answer = "123456"
                            },
                            new Problem
                            {
                                Level = 5,
                                Title = "测试题目 5",
                                Content = "<p>Test<a href=\"javascript:void(0)\" onclick=\"emmm()\">？</a></p>",
                                Script = "function emmm() { alert('123456') }",
                                Answer = "123456"
                            },
                        });

                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }
#endif
            #endregion
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseSystemfdLiveReload()
                        .UseStartup<Startup>();
                });
    }
}
