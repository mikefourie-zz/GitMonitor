// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommitRepository.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Repositories
{
    using System;
    using System.Collections.Generic;
    using GitMonitor.Models;

    public interface ICommitRepository
    {
        void FetchAll(MonitoredPathConfig m, string name);

        List<GitCommit> SearchForCommit(MonitoredPathConfig mpc, string repositoryName, string sha);
        
        List<string> SearchBranchesForCommit(MonitoredPathConfig mpc, string repositoryName, string sha, string filter);

        MonitoredPathConfig Get(MonitoredPathConfig mpc, string name, string branchName, int days);

        MonitoredPathConfig Get(MonitoredPathConfig mpc, string name, string branchName, DateTime startDateTime, DateTime endDateTime);
        
        MonitoredPath Get(MonitoredPathConfig mpc, string monitoredPathName, string repoName, string branchName, int days);

        MonitoredPath Get(MonitoredPathConfig mpc, string monitoredPathName, string repoName, string branchName, DateTime startDateTime, DateTime endDateTime);
    }
}
