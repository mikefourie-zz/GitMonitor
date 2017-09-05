// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitSearch.cs">(c) 2017 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
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
