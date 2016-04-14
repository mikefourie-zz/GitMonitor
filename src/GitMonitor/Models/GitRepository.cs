// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepository.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Models
{
    using LibGit2Sharp;

    public class GitRepository
    {
        public string Name { get; set; }

        public string FriendlyName { get; set; }

        public int CommitCount { get; set; }

        public string Branch { get; set; }

        public Repository Repository { get; set; }
    }
}
