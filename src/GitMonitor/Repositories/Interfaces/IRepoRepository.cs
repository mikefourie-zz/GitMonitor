// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepoRepository.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Repositories
{
    using System;
    using System.Collections.Generic;
    using GitMonitor.Models;

    public interface IRepoRepository
    {
        List<GitRepository> Get(MonitoredPathConfig mpc, string monitoredPathName, string repository);
    }
}
