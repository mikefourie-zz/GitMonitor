// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonitoredPathConfig.cs">(c) 2018 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Models
{
    using System;
    using System.Collections.Generic;

    public class MonitoredPathConfig
    {
        public MonitoredPathConfig()
        {
            this.MonitoredPaths = new List<MonitoredPath>();
        }

        public List<MonitoredPath> MonitoredPaths { get; set; }

        public MonitoredPath ActiveMonitoredPath { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public GitSearch Search { get; set; }

        public string DefaultDays { get; set; }

        public string DefaultRemote { get; set; }

        public string DefaultBranch { get; set; }

        public string DefaultUserNameExcludeFilter { get; set; }
    }
}
