// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitCommit.cs">(c) 2018 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Models
{
    using System;

    public class GitCommit
    {
        public string CommitUrl { get; set; }

        public string Author { get; set; }

        public string AuthorEmail { get; set; }

        public DateTime AuthorWhen { get; set; }

        public string Committer { get; set; }

        public string CommitterEmail { get; set; }

        public DateTime CommitterWhen { get; set; }

        public string Message { get; set; }

        public string Sha { get; set; }

        public bool IsMerge { get; set; }

        public string RepositoryFriendlyName { get; set; }

        public string RepositoryName { get; set; }

        public string RepositoryUrl { get; set; }
    }
}
