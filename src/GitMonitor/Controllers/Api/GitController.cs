// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitController.cs" company="FreeToDev">Copyright Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using System;
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
        private IOptions<MonitoredPathConfig> localMonitoredPathConfig;

        public GitController(ICommitRepository repository, ILogger<GitController> logger, IOptions<MonitoredPathConfig> monitoredPathConfig)
        {
            this.localRepository = repository;
            this.locallogger = logger;
            this.localMonitoredPathConfig = monitoredPathConfig;
        }

        public JsonResult Get()
        {
            int days = Convert.ToInt32(Startup.Configuration["Defaults:DefaultDays"]);
            if (days == 0)
            {
                days = -1;
            }

            if (days > 0)
            {
                days = days * -1;
            }

            var results = this.localRepository.GetDefault(this.localMonitoredPathConfig.Value, days);
            return this.Json(results);
        }

        [Route("{days:int}")]
        public JsonResult Get(int days)
        {
            var results = this.localRepository.GetDefault(this.localMonitoredPathConfig.Value, days);
            return this.Json(results);
        }

        [Route("{repoName}")]
        public JsonResult Get(string repoName)
        {
            var results = this.localRepository.Get(repoName, 0);
            return this.Json(results);
        }

        [Route("{repoName}/{branchName}")]
        public JsonResult Get(string repoName, string branchName)
        {
            var results = this.localRepository.Get(repoName, branchName, 0);
            return this.Json(results);
        }

        [Route("{repoName}/{days:int}")]
        public JsonResult Get(string repoName, int days)
        {
            var results = this.localRepository.Get(repoName, days);
            return this.Json(results);
        }
    }
}