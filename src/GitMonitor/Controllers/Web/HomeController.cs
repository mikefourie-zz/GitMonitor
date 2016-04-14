// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Controllers
{
    using GitMonitor.Models;
    using GitMonitor.Repositories;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.OptionsModel;

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
        
        public IActionResult Index(int days)
        {
            var results = this.localRepository.GetDefault(this.localMonitoredPathConfig.Value, days);
            return this.View(results);
        }

        public IActionResult Groups(int days)
        {
            var results = this.localRepository.GetGroups(this.localMonitoredPathConfig.Value, days);
            return this.View(results);
        }

        public IActionResult Fetch()
        {
            this.localRepository.FetchAll();
            return this.RedirectToAction("Index");
        }

        public IActionResult FetchGroups()
        {
            this.localRepository.FetchAll();
            return this.RedirectToAction("Groups");
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}
