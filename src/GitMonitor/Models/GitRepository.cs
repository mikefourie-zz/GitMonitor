// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepository.cs" company="FreeToDev">Mike Fourie</copyright>
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

        public string Branch { get; set; }
    }
}
