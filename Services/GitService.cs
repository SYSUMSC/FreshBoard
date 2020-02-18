using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FreshBoard.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;

namespace FreshBoard.Services
{
    public class GitTaskOptions
    {
        public string GitOrg { get; set; } = "";
        public string GitRepo { get; set; } = "";
    }
    public class GitService
    {
        private readonly ILogger<GitService> _logger;
        private readonly IOptions<GitTaskOptions> _options;
        private static readonly GitHubClient client = new GitHubClient(new ProductHeaderValue("FreshBoard"));

        public GitService(ILogger<GitService> logger, IOptions<GitTaskOptions> options)
        {
            _logger = logger;
            _options = options;
        }


        public async Task<List<GitItemModel>> GetItemsAsync(string path)
        {
            var encodedPath = path.Split("/").Select(HttpUtility.UrlEncode).Aggregate("", (accu, next) => $"{accu}/{next}");
            var contents = await client.Repository.Content.GetAllContents(_options.Value.GitOrg, _options.Value.GitRepo, encodedPath);
            return contents.Select(i => new GitItemModel { Name = i.Name, Path = i.Path, Uri = i.DownloadUrl, Size = i.Size, Type = i.Type.Value }).ToList();
        }

        public async Task<string> GetContentAsync(string uri)
        {
            throw new NotImplementedException();
        }
    }
}