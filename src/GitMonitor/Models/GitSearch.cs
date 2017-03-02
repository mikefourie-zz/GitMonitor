// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitSearch.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Models
{
    using System.Collections.Generic;

    public class GitSearch
    {
        public string Sha { get; set; }

        public List<GitCommit> Commits { get; set; }

        public List<string> Branches { get; set; }
    }
}
