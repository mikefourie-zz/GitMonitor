﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs">(c) 2018 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Export
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using GitMonitor.Models;
    using Newtonsoft.Json;

    public class Program
    {
        private static readonly Options Arguments = new Options();
        private static ExcelHelper myExcel;

        public static void Main(string[] args)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Git Monitor Exporter (c) Mike Fourie - FreeToDev");
            Console.WriteLine("--------------------------------------------------");

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please provide valid arguments, e.g. --service-endpoint [YOUR URL] --monitoredpathname default --days 180");
                return;
            }

            CommandLine.Parser.Default.ParseArguments(args, Arguments);
            if (Arguments.Append && string.IsNullOrEmpty(Arguments.FileName))
            {
                Console.WriteLine("A filename must be provided when append is true.");
                return;
            }

            if (!Arguments.Append)
            {
                myExcel = new ExcelHelper(string.Empty);
                myExcel.AddWorksheet("CommitData");
                myExcel.WriteHeaderRow("Sha,CommitUrl,Author,AuthorEmail,AuthorWhen,Committer,CommitterEmail,CommitterWhen,CommitterWhenShort,IsMerge,Message,RepositoryFriendlyName,RepositoryName,BranchName,DayOfWeek,WeekOfYear,Month,Year");
            }
            else
            {
                myExcel = new ExcelHelper(Arguments.FileName);
            }

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}  --- Export Started for {Arguments.BranchName}");
            RunAsync().Wait();
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}  --- Export Completed");
        }

        private static async Task RunAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Arguments.ServiceEndPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string urltopass;
                if (Arguments.StartDate != DateTime.MinValue)
                {
                    if (Arguments.EndDate != DateTime.MinValue)
                    {
                        urltopass = string.IsNullOrEmpty(Arguments.RepositoryName) ? $"/api/commits/{Arguments.MonitoredPathName}/{Arguments.StartDate:dd MMM yyyy}/{Arguments.EndDate:dd MMM yyyy}?branchName={Arguments.BranchName}" : $"/api/commits/{Arguments.MonitoredPathName}/{Arguments.StartDate:dd MMM yyyy}/{Arguments.EndDate:dd MMM yyyy}?repoName={Arguments.RepositoryName}&branchName={Arguments.BranchName}";
                    }
                    else
                    {
                        urltopass = string.IsNullOrEmpty(Arguments.RepositoryName) ? $"/api/commits/{Arguments.MonitoredPathName}/{Arguments.StartDate:dd MMM yyyy}?branchName={Arguments.BranchName}" : $"/api/commits/{Arguments.MonitoredPathName}/{Arguments.StartDate:dd MMM yyyy}?branchName={Arguments.BranchName}?repoName={Arguments.RepositoryName}&branchName={Arguments.BranchName}";
                    }
                }
                else
                {
                    urltopass = string.IsNullOrEmpty(Arguments.RepositoryName) ? $"/api/commits/{Arguments.MonitoredPathName}?days={Arguments.Days}&branchName={Arguments.BranchName}" : $"/api/commits/{Arguments.MonitoredPathName}?repoName={Arguments.RepositoryName}&branchName={Arguments.BranchName}&days={Arguments.Days}";
                }

                var response = client.GetAsync(urltopass).Result;
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var mi = JsonConvert.DeserializeObject<MonitoredPath>(content);
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()}  --- Processing {mi.CommitCount} Results");
                    if (!Arguments.Append)
                    {
                        myExcel.WriteHeaderRow("Sha,CommitUrl,Author,AuthorEmail,AuthorWhen,Committer,CommitterEmail,CommitterWhen,CommitterWhenShort,IsMerge,Message,RepositoryFriendlyName,RepositoryName,BranchName,DayOfWeek,WeekOfYear,Month,Year");
                    }
                    else
                    {
                        myExcel.GetWorksheet("CommitData");
                    }

                    var culture = new CultureInfo("en-US");
                    var calendar = culture.Calendar;
                    int row = 0;
                    int colcount = 18;
                    var data = new object[mi.CommitCount, colcount];
                    foreach (var commit in mi.Commits)
                    {
                        int column = 0;
                        data[row, column++] = commit.Sha;
                        data[row, column++] = commit.CommitUrl;
                        data[row, column++] = commit.Author;
                        data[row, column++] = commit.AuthorEmail;
                        data[row, column++] = commit.AuthorWhen.ToString("F");
                        data[row, column++] = commit.Committer;
                        data[row, column++] = commit.CommitterEmail;
                        data[row, column++] = commit.CommitterWhen.ToString("F");
                        data[row, column++] = commit.CommitterWhen.ToString("dd MMM yy");
                        data[row, column++] = commit.IsMerge;
                        data[row, column++] = commit.Message;
                        data[row, column++] = commit.RepositoryFriendlyName;
                        data[row, column++] = commit.RepositoryName;
                        data[row, column++] = Arguments.BranchName;
                        data[row, column++] = commit.CommitterWhen.ToString("ddd");
                        data[row, column++] = calendar.GetWeekOfYear(commit.CommitterWhen, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
                        data[row, column++] = commit.CommitterWhen.ToString("MMMM");
                        data[row, column] = commit.CommitterWhen.ToString("yyyy");
                        row++;
                    }

                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()}  --- Writing Data");
                    if (Arguments.Append)
                    {
                        myExcel.Append(data, mi.CommitCount, colcount);
                    }
                    else
                    {
                        myExcel.Write(data, mi.CommitCount, colcount);
                    }

                    string fileName = Arguments.FileName;
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        fileName = DateTime.Now.ToString("dd MMM yy hh-mm") + " ChangesetData.xlsx";
                    }

                    myExcel.SaveWorkBook(Path.Combine(Environment.CurrentDirectory, fileName), Arguments.Append);
                    myExcel.Close();
                }
            }
        }
    }
}
