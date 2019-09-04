using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FreshBoard.Services
{
    public class GitTaskOptions
    {
        public string GitRepoUrl { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";
    }
    public class GitBackgroundTask : BackgroundService
    {
        private readonly ILogger<GitBackgroundTask> _logger;
        private readonly IOptions<GitTaskOptions> _options;

        public GitBackgroundTask(ILogger<GitBackgroundTask> logger, IOptions<GitTaskOptions> options)
        {
            _logger = logger;
            _options = options;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = _options.Value;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(options.WorkingDirectory))
                {
                    Process.Start("git", $"clone \"{options.GitRepoUrl}\" \"{options.WorkingDirectory}\"").WaitForExit();
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "reset --hard",
                    WorkingDirectory = options.WorkingDirectory,
                    UseShellExecute = false
                }).WaitForExit();
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "pull",
                    WorkingDirectory = options.WorkingDirectory,
                    UseShellExecute = false
                }).WaitForExit();

                await Task.Delay(120 * 1000);
            }
        }
    }
}