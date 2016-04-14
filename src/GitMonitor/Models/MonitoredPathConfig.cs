// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonitoredPathConfig.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Models
{
    using System.Collections.Generic;

    public class MonitoredPathConfig
    {
        public MonitoredPathConfig()
        {
            this.MonitoredPaths = new List<MonitoredPath>();
        }

        public List<MonitoredPath> MonitoredPaths { get; set; }
    }
}
