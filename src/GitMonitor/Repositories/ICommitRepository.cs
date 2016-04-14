// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommitRepository.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Repositories
{
    using System.Collections.Generic;
    using GitMonitor.Models;
    
    public interface ICommitRepository
    {
        void FetchAll();

        MonitoredPath GetDefault(MonitoredPathConfig m, int days);

        MonitoredPathConfig GetGroups(MonitoredPathConfig m, int days);

        IEnumerable<MonitoredPath> Get(string repoName, int days);

        IEnumerable<MonitoredPath> Get(string repoName, string branchName, int days);
    }
}
