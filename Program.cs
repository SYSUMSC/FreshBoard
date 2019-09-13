using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
        public static Task Main(string[] args) =>
            CreateHostBuilder(args).Build().RunAsync();

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
