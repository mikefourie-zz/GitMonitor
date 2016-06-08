// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitSearchController.cs" company="FreeToDev">Copyright Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using GitMonitor.Models;
    using GitMonitor.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [Route("api/search")]
    public class GitSearchController : Controller
    {
        private readonly ILogger<GitController> locallogger;
        private readonly ICommitRepository localRepository;
        private readonly IOptions<MonitoredPathConfig> localMonitoredPathConfig;

        public GitSearchController(ICommitRepository repository, ILogger<GitController> logger, IOptions<MonitoredPathConfig> monitoredPathConfig)
        {
            this.localRepository = repository;
            this.locallogger = logger;
            this.localMonitoredPathConfig = monitoredPathConfig;
        }

        [Route("{sha}")]
        public JsonResult Get(string sha)
        {
            GitSearch search = new GitSearch
            {
                Sha = sha,
                Commits = this.localRepository.SearchForCommit(this.localMonitoredPathConfig.Value, string.Empty, sha)
            };
            return this.Json(search);
        }

        [Route("{pathName}/{sha}")]
        public JsonResult Get(string pathName, string sha)
        {
            GitSearch search = new GitSearch
            {
                Sha = sha,
                Commits = this.localRepository.SearchForCommit(this.localMonitoredPathConfig.Value, pathName, sha)
            };
            return this.Json(search);
        }
    }
}