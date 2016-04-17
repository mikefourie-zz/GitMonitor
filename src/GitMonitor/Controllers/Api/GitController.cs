// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitController.cs" company="FreeToDev">Copyright Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using GitMonitor.Models;
    using GitMonitor.Repositories;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.OptionsModel;

    [Route("api/commits")]
    public class GitController : Controller
    {
        private readonly ILogger<GitController> locallogger;
        private readonly ICommitRepository localRepository;
        private readonly IOptions<MonitoredPathConfig> localMonitoredPathConfig;

        public GitController(ICommitRepository repository, ILogger<GitController> logger, IOptions<MonitoredPathConfig> monitoredPathConfig)
        {
            this.localRepository = repository;
            this.locallogger = logger;
            this.localMonitoredPathConfig = monitoredPathConfig;
        }

        [Route("{days:int?}")]
        public JsonResult Get(int days)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, string.Empty, string.Empty, string.Empty, days);
            return this.Json(results);
        }

        [Route("{pathName}/{days:int?}")]
        public JsonResult Get(string pathName, int days)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, pathName, string.Empty, string.Empty, days);
            return this.Json(results);
        }

        [Route("{pathName}/{repoName}/{days:int?}")]
        public JsonResult Get(string pathName, string repoName, int days)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, pathName, repoName, string.Empty, days);
            return this.Json(results);
        }

        [Route("{pathName}")]
        public JsonResult Get(string pathName, string repoName, string branchName, int days)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, pathName, repoName, branchName, days);
            return this.Json(results);
        }
    }
}