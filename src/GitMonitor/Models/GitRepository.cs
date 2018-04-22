// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepository.cs">(c) 2018 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Models
{
    public class GitRepository
    {
        public string Name { get; set; }

        public string FriendlyName { get; set; }

        public string CommitUrl { get; set; }

        public bool AllowFetch { get; set; }

        public int CommitCount { get; set; }

        public GitCommit LastCommit { get; set; }

        public int BranchCount { get; set; }

        public string Branch { get; set; }
    }
}
