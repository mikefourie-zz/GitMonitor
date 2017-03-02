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

        [Route("{monitoredPathName}/{sha}")]
        public JsonResult Get(string monitoredPathName, string sha)
        {
            GitSearch search = new GitSearch
            {
                Sha = sha,
                Commits = this.localRepository.SearchForCommit(this.localMonitoredPathConfig.Value, monitoredPathName, sha)
            };
            return this.Json(search);
        }

        [Route("branches/{repositoryName}/{sha}")]
        public JsonResult GetBranches(string repositoryName, string sha)
        {
            GitSearch search = new GitSearch
            {
                Sha = sha,
                Branches = this.localRepository.SearchBranchesForCommit(this.localMonitoredPathConfig.Value, repositoryName, sha)
            };
            return this.Json(search);
        }
    }
}