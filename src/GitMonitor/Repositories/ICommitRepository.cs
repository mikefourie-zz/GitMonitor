// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommitRepository.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Repositories
{
    using System.Collections.Generic;
    using GitMonitor.Models;

    public interface ICommitRepository
    {
        void FetchAll(MonitoredPathConfig m, string name);

        List<GitCommit> SearchForCommit(MonitoredPathConfig mpc, string monitoredPathName, string sha);

        MonitoredPathConfig Get(MonitoredPathConfig m, string name, string branchName, int days);

        MonitoredPath Get(MonitoredPathConfig m, string pathName, string repoName, string branchName, int days);
    }
}
