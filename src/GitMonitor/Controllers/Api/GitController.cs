// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitController.cs" company="FreeToDev">Copyright Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using System;
    using GitMonitor.Models;
    using GitMonitor.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

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

        [Route("{monitoredPathName}/{days:int?}")]
        public JsonResult Get(string monitoredPathName, int days)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, monitoredPathName, string.Empty, string.Empty, days);
            return this.Json(results);
        }

        [Route("{monitoredPathName}/{repoName}/{days:int?}")]
        public JsonResult Get(string monitoredPathName, string repoName, int days)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, monitoredPathName, repoName, string.Empty, days);
            return this.Json(results);
        }

        [Route("{monitoredPathName}")]
        public JsonResult Get(string monitoredPathName, string repoName, string branchName, int days)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, monitoredPathName, repoName, branchName, days);
            return this.Json(results);
        }

        [Route("{startDateTime:DateTime}")]
        public JsonResult Get(DateTime startDateTime)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, string.Empty, string.Empty, string.Empty, startDateTime, DateTime.UtcNow);
            return this.Json(results);
        }

        [Route("{startDateTime:DateTime}/{endDateTime:DateTime}")]
        public JsonResult Get(DateTime startDateTime, DateTime endDateTime)
        {
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, string.Empty, string.Empty, string.Empty, startDateTime, endDateTime);
            return this.Json(results);
        }
    }
}