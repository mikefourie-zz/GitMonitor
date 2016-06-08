// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommitRepository.cs" company="FreeToDev">Mike Fourie</copyright>
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

    public class CommitRepository : ICommitRepository
    {
        private readonly ILogger<CommitRepository> locallogger;

        public CommitRepository(ILogger<CommitRepository> logger)
        {
            this.locallogger = logger;
        }

        public MonitoredPathConfig Get(MonitoredPathConfig mpc, string name, string branchName, int days)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (string.Compare(mp.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        mpc.ActiveMonitoredPath = this.GetMonitoredPath(mp, string.Empty, branchName, days);
                    }
                }

                return mpc;
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
            }

            return null;
        }

        public MonitoredPath Get(MonitoredPathConfig mpc, string pathName, string repoName, string branchName, int days)
        {
            MonitoredPath nmp = new MonitoredPath();
            try
            {
                if (string.IsNullOrWhiteSpace(pathName))
                {
                    pathName = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (mp.Name.ToLower() == pathName.ToLower())
                    {
                        nmp = this.GetMonitoredPath(mp, repoName, branchName, days);
                    }
                }              
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
                return null;
            }

            return nmp;
        }

        public List<GitCommit> SearchForCommit(MonitoredPathConfig mpc, string monitoredPathName, string sha)
        {
            List<GitCommit> commits = new List<GitCommit>();

            try
            {
                if (string.IsNullOrWhiteSpace(monitoredPathName))
                {
                    monitoredPathName = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (string.Compare(mp.Name, monitoredPathName, StringComparison.OrdinalIgnoreCase) == 0 || monitoredPathName == "*")
                    {
                        DirectoryInfo[] directoriesToScan = new DirectoryInfo(mp.Path).GetDirectories("*", SearchOption.TopDirectoryOnly);
                        foreach (DirectoryInfo dir in directoriesToScan)
                        {
                            try
                            {
                                using (var repo = new Repository(dir.FullName))
                                {
                                    try
                                    {
                                        Commit com = repo.Commits.First(c => c.Sha.StartsWith(sha));
                                        if (com != null)
                                        {
                                            GitRepository gitrepo = new GitRepository();

                                            foreach (var repo1 in mp.Repositories)
                                            {
                                                if (string.Compare(repo1.Name, dir.Name, StringComparison.OrdinalIgnoreCase) == 0)
                                                {
                                                    gitrepo = new GitRepository
                                                    {
                                                        AllowFetch = repo1.AllowFetch,
                                                        CommitUrl = repo1.CommitUrl,
                                                        FriendlyName = string.IsNullOrWhiteSpace(repo1.FriendlyName) ? dir.Name : repo1.FriendlyName,
                                                        Name = dir.Name
                                                    };
                                                }
                                            }

                                            string url = string.IsNullOrWhiteSpace(gitrepo.CommitUrl) ? string.Empty : string.Format($"{gitrepo.CommitUrl}{com.Sha}");
                                            string repositoryUrl = string.Empty;
                                            if (repo.Network.Remotes?["origin"] != null)
                                            {
                                                repositoryUrl = repo.Network.Remotes["origin"].Url;
                                            }

                                            commits.Add(new GitCommit
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
                                            });
                                        }
                                    }
                                    catch
                                    {
                                        // nothing
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // do nothing
                            }
                        }
                    }
                }

                return commits;
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
            }

            return commits;
        }

        public void FetchAll(MonitoredPathConfig mpc, string pathName)
        {
            if (string.IsNullOrWhiteSpace(pathName))
            {
                pathName = "default";
            }

            foreach (MonitoredPath mp in mpc.MonitoredPaths)
            {
                if (mp.Name.ToLower() == pathName.ToLower())
                {
                    if (mp.AllowFetch)
                    {
                        try
                        {
                            DirectoryInfo[] directoriesToScan;
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

                            foreach (var dir in directoriesToScan)
                            {
                                try
                                {
                                    using (var repo = new Repository(dir.FullName))
                                    {
                                        Remote remote = repo.Network.Remotes["origin"];
                                        repo.Network.Fetch(remote);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.locallogger.LogError($"Error: {ex.Message}");
                                    
                                    // swallow
                                }                      
                            }
                        }
                        catch (Exception ex)
                        {
                            this.locallogger.LogError($"Error: {ex.Message}");
                            
                            // swallow
                        }
                    }
                }
            }
        }

        private MonitoredPath GetMonitoredPath(MonitoredPath monitoredPath, string repository, string branchName, int days)
        {
            List<GitCommit> commits = new List<GitCommit>();

            // if (days == 0)
            // {
            //    days = monitoredPath.Days == 0 ? Convert.ToInt32(Startup.Configuration["Defaults:DefaultDays"]) : monitoredPath.Days;
            // }
            DirectoryInfo[] directoriesToScan;
            if (!string.IsNullOrWhiteSpace(repository))
            {
                directoriesToScan = new DirectoryInfo(monitoredPath.Path).GetDirectories(repository, SearchOption.TopDirectoryOnly);
            }
            else
            {
                if (monitoredPath.AllFolders)
                {
                    directoriesToScan = new DirectoryInfo(monitoredPath.Path).GetDirectories("*", SearchOption.TopDirectoryOnly);
                }
                else
                {
                    directoriesToScan = new DirectoryInfo[monitoredPath.Repositories.Count];
                    int i = 0;
                    foreach (var dir in monitoredPath.Repositories)
                    {
                        directoriesToScan[i++] = new DirectoryInfo(Path.Combine(monitoredPath.Path, dir.Name));
                    }
                }
            }

            // if (days == 0)
            // {
            //    days = Convert.ToInt32(Startup.Configuration["Defaults:DefaultDays"]);
            // }
            if (days > 0)
            {
                days = days * -1;
            }

            MonitoredPath newmonitoredPath = new MonitoredPath();
            foreach (var dir in directoriesToScan)
            {
                try
                {
                    GitRepository gitrepo = this.TryGetRepo(monitoredPath, dir.Name);
                    using (var repo = new Repository(dir.FullName))
                    {
                        DateTime startDate = DateTime.Now.AddDays(days);
                        int commitCount = 0;
                        if (string.IsNullOrEmpty(branchName))
                        {
                            branchName = "master";
                        }

                        string branch = repo.Info.IsBare ? branchName : $"origin/{branchName}";
                        gitrepo.Branch = branch;
                        foreach (
                            LibGit2Sharp.Commit com in
                                repo.Branches[branch].Commits.Where(s => s.Committer.When >= startDate)
                                    .OrderByDescending(s => s.Author.When))
                        {
                            if (!monitoredPath.IncludeMergeCommits)
                            {
                                // filter out merge commits
                                if (com.Parents.Count() > 1)
                                {
                                      continue;
                                }
                            }

                            // string[] nameexclusions = Startup.Configuration["Defaults:DefaultUserNameExcludeFilter"].Split(',');
                            // if (nameexclusions.Any(name => com.Author.Name.Contains(name)))
                            // {
                            //    continue;
                            // }
                            string url = string.IsNullOrWhiteSpace(gitrepo.CommitUrl)
                                ? string.Empty
                                : string.Format($"{gitrepo.CommitUrl}{com.Sha}");
                            string repositoryUrl = string.Empty;
                            if (repo.Network.Remotes?["origin"] != null)
                            {
                                repositoryUrl = repo.Network.Remotes["origin"].Url;
                            }

                            commits.Add(new GitCommit
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
                            });
                            commitCount++;
                        }

                        gitrepo.CommitCount = commitCount;
                        newmonitoredPath.Repositories.Add(gitrepo);
                        newmonitoredPath.AllowFetch = monitoredPath.AllowFetch;
                    }

                    newmonitoredPath.Name = monitoredPath.Name;
                    newmonitoredPath.AllowFetch = monitoredPath.AllowFetch;
                    newmonitoredPath.AllFolders = monitoredPath.AllFolders;
                    newmonitoredPath.Days = days;
                    newmonitoredPath.Path = monitoredPath.Path;
                    newmonitoredPath.CommitCount = commits.Count;
                    newmonitoredPath.Commits = commits;
                    newmonitoredPath.Commits.Sort((x, y) => -DateTime.Compare(x.CommitterWhen, y.CommitterWhen));
                }
                catch (Exception ex)
                {
                    this.locallogger.LogError("GetMonitoredItem Bad - ", ex);
                }
            }

            return newmonitoredPath;
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
