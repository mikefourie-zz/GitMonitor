// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommitRepository.cs">(c) 2018 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
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

        public MonitoredPathConfig Get(MonitoredPathConfig mpc, string monitoredPathName, string branchName, int days)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(monitoredPathName))
                {
                    monitoredPathName = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (string.Compare(mp.Name, monitoredPathName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        mpc.ActiveMonitoredPath = this.GetMonitoredPath(mpc, mp, string.Empty, branchName, days);
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

        public MonitoredPathConfig Get(MonitoredPathConfig mpc, string monitoredPathName, string branchName, DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(monitoredPathName))
                {
                    monitoredPathName = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (string.Compare(mp.Name, monitoredPathName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        mpc.ActiveMonitoredPath = this.GetMonitoredPath(mpc, mp, string.Empty, branchName, startDateTime, endDateTime);
                    }
                }

                mpc.StartDateTime = startDateTime;
                mpc.EndDateTime = endDateTime;
                return mpc;
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
            }

            return null;
        }

        public MonitoredPath Get(MonitoredPathConfig mpc, string monitoredPathName, string repoName, string branchName, int days)
        {
            MonitoredPath newmonitoredPath = new MonitoredPath();
            try
            {
                if (string.IsNullOrWhiteSpace(monitoredPathName))
                {
                    monitoredPathName = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (mp.Name.ToLower() == monitoredPathName.ToLower())
                    {
                        newmonitoredPath = this.GetMonitoredPath(mpc, mp, repoName, branchName, days);
                    }
                }              
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
                return null;
            }

            return newmonitoredPath;
        }

        public MonitoredPath Get(MonitoredPathConfig mpc, string monitoredPathName, string repoName, string branchName, DateTime startDateTime, DateTime endDateTime)
        {
            MonitoredPath newmonitoredPath = new MonitoredPath();
            try
            {
                if (string.IsNullOrWhiteSpace(monitoredPathName))
                {
                    monitoredPathName = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (mp.Name.ToLower() == monitoredPathName.ToLower())
                    {
                        newmonitoredPath = this.GetMonitoredPath(mpc, mp, repoName, branchName, startDateTime, endDateTime);
                    }
                }
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
                return null;
            }

            return newmonitoredPath;
        }

        public List<string> SearchBranchesForCommit(MonitoredPathConfig monitoredPathConfig, string repositoryName, string sha, string filter)
        {
            List<string> branches = new List<string>();
            bool found = false;
            try
            {
                foreach (MonitoredPath mp in monitoredPathConfig.MonitoredPaths)
                {
                    DirectoryInfo[] directoriesToScan = new DirectoryInfo(mp.Path).GetDirectories("*", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo dir in directoriesToScan)
                    {
                        if (string.Compare(dir.Name, repositoryName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            Repository r = new Repository($"{mp.Path}\\{repositoryName}");
                            IEnumerable<Branch> b = this.ListBranchesContaininingCommit(r, sha, filter);

                            foreach (Branch i in b)
                            {
                                branches.Add(i.FriendlyName);
                            }

                            found = true;
                            break;
                        }

                        if (found)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
            }

            return branches;
        }

        public List<GitCommit> SearchForCommit(MonitoredPathConfig monitoredPathConfig, string monitoredPathName, string sha)
        {
            List<GitCommit> commits = new List<GitCommit>();

            try
            {
                if (string.IsNullOrWhiteSpace(monitoredPathName))
                {
                    monitoredPathName = "default";
                }

                foreach (MonitoredPath mp in monitoredPathConfig.MonitoredPaths)
                {
                    if (string.Compare(mp.Name, monitoredPathName, StringComparison.OrdinalIgnoreCase) == 0 || monitoredPathName == "*")
                    {
                        DirectoryInfo[] directoriesToScan = new DirectoryInfo(mp.Path).GetDirectories("*", SearchOption.TopDirectoryOnly);
                        foreach (DirectoryInfo dir in directoriesToScan)
                        {
                            try
                            {
                                using (Repository repo = new Repository(dir.FullName))
                                {
                                    try
                                    {
                                        Commit com = repo.Lookup<Commit>(sha);
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

        public void FetchAll(MonitoredPathConfig mpc, string monitoredPathName)
        {
            if (string.IsNullOrWhiteSpace(monitoredPathName))
            {
                monitoredPathName = "default";
            }

            foreach (MonitoredPath mp in mpc.MonitoredPaths)
            {
                if (mp.Name.ToLower() == monitoredPathName.ToLower())
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
                                        string logMessage = string.Empty;
                                        Remote remote = repo.Network.Remotes["origin"];
                                        IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                                        Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);

                                        this.locallogger.LogInformation($"{logMessage}");
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

        public MonitoredPath Get(MonitoredPathConfig mpc, string monitoredPathName, string repoName, string branchName, string commitId)
        {
            MonitoredPath newmonitoredPath = new MonitoredPath();
            try
            {
                if (string.IsNullOrWhiteSpace(monitoredPathName))
                {
                    monitoredPathName = "default";
                }

                foreach (MonitoredPath mp in mpc.MonitoredPaths)
                {
                    if (mp.Name.ToLower() == monitoredPathName.ToLower())
                    {
                        DirectoryInfo[] directoriesToScan;

                        if (!string.IsNullOrWhiteSpace(repoName))
                        {
                            directoriesToScan = new DirectoryInfo(mp.Path).GetDirectories(repoName, SearchOption.TopDirectoryOnly);
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

                        newmonitoredPath = this.GetMonitoredPath(commitId, mp, directoriesToScan, branchName);
                    }
                }
            }
            catch (Exception ex)
            {
                this.locallogger.LogError("Bad - ", ex);
                return null;
            }

            return newmonitoredPath;
        }

        private IEnumerable<Branch> ListBranchesContaininingCommit(Repository repo, string commitSha, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                foreach (var branch in repo.Branches)
                {
                    var commits = branch.Commits.Where(c => c.Sha == commitSha);

                    if (!commits.Any())
                    {
                        continue;
                    }

                    yield return branch;
                }
            }
            else
            {
                foreach (var branch in repo.Branches.Where(n => n.FriendlyName.Contains(filter)))
                {
                    var commits = branch.Commits.Where(c => c.Sha == commitSha);

                    if (!commits.Any())
                    {
                        continue;
                    }

                    yield return branch;
                }
            }
        }

        private MonitoredPath GetMonitoredPath(MonitoredPathConfig monitoredPathConfig, MonitoredPath monitoredPath, string repository, string branchName, DateTime startDateTime, DateTime endDateTime)
        {
            List<GitCommit> commits = new List<GitCommit>();
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

            MonitoredPath newmonitoredPath = new MonitoredPath();
            foreach (var dir in directoriesToScan)
            {
                try
                {
                    GitRepository gitrepo = this.TryGetRepo(monitoredPath, dir.Name);
                    using (var repo = new Repository(dir.FullName))
                    {
                        int commitCount = 0;
                        if (string.IsNullOrEmpty(branchName))
                        {
                            branchName = "master";
                        }

                        string branch = repo.Info.IsBare ? branchName : $"origin/{branchName}";
                        gitrepo.Branch = branch;

                        foreach (Commit com in repo.Branches[branch].Commits.Where(s => s.Committer.When >= startDateTime && s.Committer.When <= endDateTime).OrderByDescending(s => s.Author.When))
                        {
                            if (!monitoredPath.IncludeMergeCommits)
                            {
                                // filter out merge commits
                                if (com.Parents.Count() > 1)
                                {
                                      continue;
                                }
                            }

                            string[] nameexclusions = monitoredPathConfig.DefaultUserNameExcludeFilter.Split(',');
                            if (nameexclusions.Any(name => com.Author.Name.Contains(name)))
                            {
                                continue;
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
                            commitCount++;
                        }

                        gitrepo.CommitCount = commitCount;
                        newmonitoredPath.Repositories.Add(gitrepo);
                        newmonitoredPath.AllowFetch = monitoredPath.AllowFetch;
                    }

                    newmonitoredPath.Name = monitoredPath.Name;
                    newmonitoredPath.AllowFetch = monitoredPath.AllowFetch;
                    newmonitoredPath.AllFolders = monitoredPath.AllFolders;
                    newmonitoredPath.StartDateTime = startDateTime;
                    newmonitoredPath.EndDateTime = endDateTime;
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

        private MonitoredPath GetMonitoredPath(MonitoredPathConfig monitoredPathConfig, MonitoredPath monitoredPath, string repository, string branchName, int days)
        {
            List<GitCommit> commits = new List<GitCommit>();
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

            if (days == 0)
            {
                days = monitoredPath.Days == 0 ? Convert.ToInt32(monitoredPathConfig.DefaultDays) : monitoredPath.Days;
            }

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
                        foreach (Commit com in repo.Branches[branch].Commits.Where(s => s.Committer.When >= startDate).OrderByDescending(s => s.Author.When))
                        {
                            if (!monitoredPath.IncludeMergeCommits)
                            {
                                // filter out merge commits
                                if (com.Parents.Count() > 1)
                                {
                                    continue;
                                }
                            }

                            string[] nameexclusions = monitoredPathConfig.DefaultUserNameExcludeFilter.Split(',');
                            if (nameexclusions.Any(name => com.Author.Name.Contains(name)))
                            {
                                continue;
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

        private MonitoredPath GetMonitoredPath(string commitId, MonitoredPath monitoredPath, DirectoryInfo[] directoriesToScan, string branchName)
        {
            MonitoredPath newmonitoredPath = new MonitoredPath();
            List<GitCommit> commits = new List<GitCommit>();
            foreach (DirectoryInfo dir in directoriesToScan)
            {
                try
                {
                    GitRepository gitrepo = this.TryGetRepo(monitoredPath, dir.Name);
                    using (Repository repo = new Repository(dir.FullName))
                    {
                        try
                        {
                            string branch = repo.Info.IsBare ? branchName : $"origin/{branchName}";
                            gitrepo.Branch = branch;

                            int commitCount = 0;
                            Commit com = repo.Lookup<Commit>(commitId);

                            if (com != null)
                            {
                                var comFilter = new CommitFilter
                                {
                                    IncludeReachableFrom = branch,
                                    ExcludeReachableFrom = com
                                };

                                var coms = repo.Commits.QueryBy(comFilter).OrderBy(s => s.Author.When);

                                string repositoryUrl = string.Empty;
                                if (repo.Network.Remotes?["origin"] != null)
                                {
                                    repositoryUrl = repo.Network.Remotes["origin"].Url;
                                }

                                foreach (Commit cm in coms)
                                {
                                    if (!monitoredPath.IncludeMergeCommits)
                                    {
                                        // filter out merge commits
                                        if (com.Parents.Count() > 1)
                                        {
                                            continue;
                                        }
                                    }

                                    commits.Add(new GitCommit
                                    {
                                        Author = cm.Author.Name,
                                        AuthorEmail = string.IsNullOrWhiteSpace(com.Author.Email) ? string.Empty : cm.Author.Email,
                                        AuthorWhen = cm.Author.When.UtcDateTime,
                                        Committer = cm.Committer.Name,
                                        CommitterEmail = string.IsNullOrWhiteSpace(com.Committer.Email) ? string.Empty : cm.Committer.Email,
                                        CommitterWhen = cm.Committer.When.UtcDateTime,
                                        Sha = cm.Sha,
                                        Message = cm.Message,
                                        RepositoryFriendlyName = gitrepo.FriendlyName,
                                        RepositoryName = dir.Name,
                                        RepositoryUrl = repositoryUrl,
                                        CommitUrl = string.IsNullOrWhiteSpace(gitrepo.CommitUrl) ? string.Empty : string.Format($"{gitrepo.CommitUrl}{cm.Sha}"),
                                        IsMerge = com.Parents.Count() > 1
                                    });
                                    commitCount++;
                                }

                                gitrepo.CommitCount = commitCount;
                                newmonitoredPath.Repositories.Add(gitrepo);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.locallogger.LogError("GetMonitoredPath Bad - ", ex);
                        }

                        newmonitoredPath.Name = monitoredPath.Name;
                        newmonitoredPath.AllowFetch = monitoredPath.AllowFetch;
                        newmonitoredPath.AllFolders = monitoredPath.AllFolders;
                        newmonitoredPath.Path = monitoredPath.Path;
                        newmonitoredPath.CommitCount = commits.Count;
                        newmonitoredPath.Commits = commits;
                    }
                }
                catch (Exception ex)
                {
                    this.locallogger.LogError("GetMonitoredPath Bad - ", ex);
                }
            }

            return newmonitoredPath;
        }
    }
}