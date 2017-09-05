// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepoRepository.cs">(c) 2017 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GitMonitor.Models;
    using LibGit2Sharp;
    using Microsoft.Extensions.Logging;

    public class RepoRepository : IRepoRepository
    {
        private readonly ILogger<RepoRepository> locallogger;

        public RepoRepository(ILogger<RepoRepository> logger)
        {
            this.locallogger = logger;
        }

        public List<GitRepository> Get(MonitoredPathConfig mpc, string monitoredPathName, string repository)
        {
            List<GitRepository> repos = new List<GitRepository>();

            if (string.IsNullOrWhiteSpace(monitoredPathName))
            {
                monitoredPathName = "default";
            }

            foreach (MonitoredPath mp in mpc.MonitoredPaths)
            {
                if (string.Compare(mp.Name, monitoredPathName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    DirectoryInfo[] directoriesToScan;
                    if (!string.IsNullOrWhiteSpace(repository))
                    {
                        directoriesToScan = new DirectoryInfo(mp.Path).GetDirectories(repository, SearchOption.TopDirectoryOnly);
                    }
                    else
                    {
                        if (mp.AllFolders)
                        {
                            directoriesToScan = new DirectoryInfo(mp.Path).GetDirectories("*", SearchOption.TopDirectoryOnly);
                        }
                        else
                        {
                            directoriesToScan = new DirectoryInfo[mp.Repositories.Count];
                            int i = 0;
                            foreach (var dir in mp.Repositories)
                            {
                                directoriesToScan[i++] = new DirectoryInfo(Path.Combine(mp.Path, dir.Name));
                            }
                        }
                    }

                    foreach (var dir in directoriesToScan)
                    {
                        try
                        {
                            GitRepository gitrepo = this.TryGetRepo(mp, dir.Name);
                            using (var repo = new Repository(dir.FullName))
                            {
                                gitrepo.BranchCount = repo.Branches.Count();
                                Commit com = repo.Head.Tip;
                                string url = string.IsNullOrWhiteSpace(gitrepo.CommitUrl) ? string.Empty : string.Format($"{gitrepo.CommitUrl}{com.Sha}");

                                string repositoryUrl = string.Empty;

                                if (repo.Network.Remotes?["origin"] != null)
                                {
                                    repositoryUrl = repo.Network.Remotes["origin"].Url;
                                }

                                gitrepo.LastCommit = new GitCommit
                                {
                                    Author = com.Author.Name,
                                    AuthorEmail = string.IsNullOrWhiteSpace(com.Author.Email) ? string.Empty : com.Author.Email,
                                    AuthorWhen = com.Author.When.UtcDateTime,
                                    Committer = com.Committer.Name,
                                    CommitterEmail = string.IsNullOrWhiteSpace(com.Committer.Email) ? string.Empty : com.Committer.Email,
                                    CommitterWhen = com.Committer.When.UtcDateTime,
                                    Sha = com.Sha,
                                    Message = com.Message,
                                    RepositoryFriendlyName = gitrepo.FriendlyName,
                                    RepositoryName = dir.Name,
                                    RepositoryUrl = repositoryUrl,
                                    CommitUrl = url,
                                    IsMerge = com.Parents.Count() > 1
                                };

                                repos.Add(gitrepo);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.locallogger.LogError("GetMonitoredItem Bad - ", ex);
                        }
                    }
                }
            }

            return repos;
        }

        private GitRepository TryGetRepo(MonitoredPath monitoredPath, string directoryName)
        {
            GitRepository r = new GitRepository { Name = directoryName, FriendlyName = directoryName };
            if (monitoredPath.Repositories.Any())
            {
                foreach (var repo in monitoredPath.Repositories)
                {
                    if (string.Compare(repo.Name, directoryName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        r = new GitRepository
                        {
                            AllowFetch = repo.AllowFetch, CommitUrl = repo.CommitUrl,
                            FriendlyName = string.IsNullOrWhiteSpace(repo.FriendlyName) ? directoryName : repo.FriendlyName,
                            Name = directoryName
                        };
                    }
                }
            }

            return r;
        }
    }
}
