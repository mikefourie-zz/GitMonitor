// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitBranchSearchController.cs" company="FreeToDev">Copyright Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using GitMonitor.Models;
    using GitMonitor.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [Route("api/branches")]
    public class GitBranchSearchController : Controller
    {
        private readonly ILogger<GitController> locallogger;
        private readonly ICommitRepository localRepository;
        private readonly IOptions<MonitoredPathConfig> localMonitoredPathConfig;

        public GitBranchSearchController(ICommitRepository repository, ILogger<GitController> logger, IOptions<MonitoredPathConfig> monitoredPathConfig)
        {
            this.localRepository = repository;
            this.locallogger = logger;
            this.localMonitoredPathConfig = monitoredPathConfig;
        }
        

        [Route("{repositoryName}/{sha}")]
        public JsonResult GetBranches(string repositoryName, string sha)
        {
            GitSearch search = new GitSearch
            {
                Sha = sha,
                Branches = this.localRepository.SearchBranchesForCommit(this.localMonitoredPathConfig.Value, repositoryName, sha, string.Empty)
            };

            return this.Json(search);
        }

        [Route("{repositoryName}/{sha}/{filter}")]
        public JsonResult GetBranches(string repositoryName, string sha, string filter)
        {
            GitSearch search = new GitSearch
            {
                Sha = sha,
                Branches = this.localRepository.SearchBranchesForCommit(this.localMonitoredPathConfig.Value, repositoryName, sha, filter)
            };
            return this.Json(search);
        }
    }
}