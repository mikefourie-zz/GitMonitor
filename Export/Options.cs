// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs">(c) 2017 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Export
{
    using CommandLine;

    public class Options
    {
        [Option('s', "service-endpoint", HelpText = "The service endpoint to execute rest calls against", Required = true)]
        public string ServiceEndPoint { get; set; }

        [Option('d', "days", HelpText = "The number of days to retrieve", Required = false, DefaultValue = 10)]
        public int Days { get; set; }

        [Option('r', "repositoryname", HelpText = "The name of the repository to retrieve", Required = false)]
        public string RepositoryName { get; set; }

        [Option('b', "branchname", HelpText = "The name of the branch to retrieve", Required = false, DefaultValue = "master")]
        public string BranchName { get; set; }
        
        [Option('f', "filename", HelpText = "The name of the file to save results to", Required = false)]
        public string FileName { get; set; }

        [Option('m', "monitoredpathname", HelpText = "The name of the monitored path to query", Required = false, DefaultValue = "default")]
        public string MonitoredPathName { get; set; }
    }
}
