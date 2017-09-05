// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepoRepository.cs">(c) 2017 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
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
