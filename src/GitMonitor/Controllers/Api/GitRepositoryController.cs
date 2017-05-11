// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepositoryController.cs" company="FreeToDev">Copyright Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using System;
    using GitMonitor.Models;
    using GitMonitor.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [Route("api/repositories")]
    public class GitRepositoryController : Controller
    {
        private readonly ILogger<GitRepositoryController> locallogger;
        private readonly IRepoRepository localRepoRepository;
        private readonly IOptions<MonitoredPathConfig> localMonitoredPathConfig;

        public GitRepositoryController(IRepoRepository repository, ILogger<GitRepositoryController> logger, IOptions<MonitoredPathConfig> monitoredPathConfig)
        {
            this.localRepoRepository = repository;
            this.locallogger = logger;
            this.localMonitoredPathConfig = monitoredPathConfig;
        }

        [Route("{monitoredPathName}")]
        public JsonResult Get(string monitoredPathName)
        {
            var results = this.localRepoRepository.Get(this.localMonitoredPathConfig.Value, monitoredPathName, string.Empty);
            return this.Json(results);
        }

        [Route("{monitoredPathName}/{repoName}")]
        public JsonResult Get(string monitoredPathName, string repoName)
        {
            var results = this.localRepoRepository.Get(this.localMonitoredPathConfig.Value, monitoredPathName, repoName);
            return this.Json(results);
        }
    }
}