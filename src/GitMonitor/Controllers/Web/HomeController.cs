// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
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
        
        public IActionResult Index(string name, string branchName, int days)
        {
            this.ViewData["MPName"] = name;
            this.ViewData["MPDays"] = days;
            var results = this.localRepository.Get(this.localMonitoredPathConfig.Value, name, branchName, days);
            return this.View(results);
        }

        public IActionResult Fetch(string name, int days)
        {
            this.localRepository.FetchAll(this.localMonitoredPathConfig.Value, name);
            return this.RedirectToAction("Index", new { name, days });
        }

        public IActionResult Search(string monitoredPathName, string sha)
        {
            GitSearch search = new GitSearch();
            if (!string.IsNullOrEmpty(sha))
            {
                search.Sha = sha;
                search.Commits = this.localRepository.SearchForCommit(this.localMonitoredPathConfig.Value, monitoredPathName, sha);
            }

            return this.View(search);
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}
