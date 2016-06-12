// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using System;
    using GitMonitor.Models;
    using GitMonitor.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> locallogger;
        private readonly ICommitRepository localRepository;
        private readonly IOptions<MonitoredPathConfig> localMonitoredPathConfig;

        public HomeController(ICommitRepository repository, ILogger<HomeController> logger, IOptions<MonitoredPathConfig> monitoredPathConfig)
        {
            this.localRepository = repository;
            this.locallogger = logger;
            this.localMonitoredPathConfig = monitoredPathConfig;
        }

        public IActionResult Index(string monitoredPathName, string branchName, int days, DateTime startDateTime, DateTime endDateTime, string sha)
        {
            this.ViewData["MPName"] = monitoredPathName;
            this.localMonitoredPathConfig.Value.Search = null;
            if (!string.IsNullOrEmpty(sha))
            {
                GitSearch search = new GitSearch();
                if (!string.IsNullOrEmpty(sha))
                {
                    search.Sha = sha;
                    search.Commits = this.localRepository.SearchForCommit(this.localMonitoredPathConfig.Value, "*", sha);
                }

                MonitoredPathConfig mpc = this.localMonitoredPathConfig.Value;
                mpc.Search = search;
                return this.View(mpc);
            }

            if (startDateTime == DateTime.MinValue)
            {
                this.ViewData["MPDays"] = days;
                var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, monitoredPathName, branchName, days);
                return this.View(results);
            }
            else
            {
                var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, monitoredPathName, branchName, startDateTime, endDateTime);
                return this.View(results);
            }
        }

        public IActionResult Fetch(string name, int days)
        {
            this.localRepository.FetchAll(this.localMonitoredPathConfig.Value, name);
            return this.RedirectToAction("Index", new { name, days });
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}
