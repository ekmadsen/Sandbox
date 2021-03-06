﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ErikTheCoder.ServiceContract;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.AsyncConcurrent
{
    public static class Program
    {
        private const int _getUserMsec = 75;
        private const int _getFileLogsMsecPerDay = 150;
        private const int _getEventLogsMsecPerDay = 50;
        private const int _getDatabaseLogsMsecPerDay = 100;
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();


        public static async Task Main(string[] Arguments)
        {
            try
            {
                await RunAsync(Arguments);
            }
            catch (Exception exception)
            {
                ConsoleWriter.WriteLine(_stopwatch, exception.GetSummary(true, true), ConsoleColor.Red);
            }
        }


        private static async Task RunAsync(IReadOnlyList<string> Arguments)
        {
            var (computerName, days, createReportAsync) = ParseCommandLine(Arguments);
            // ReSharper disable once UnusedVariable
            var pcReport = await createReportAsync(computerName, days);
        }


        private static async Task<PcReport> CreateReportSequentiallyAsync(string ComputerName, int Days)
        {
            var pcReport = new PcReport
            {
                ComputerName = ComputerName,
                Days = Days
            };
            await AddUserAsync(pcReport);
            await AddFileLogsAsync(pcReport);
            await AddEventLogsAsync(pcReport);
            await AddDatabaseLogsAsync(pcReport);
            return pcReport;
        }


        private static async Task<PcReport> CreateReportConcurrentlyRaceConditionAsync(string ComputerName, int Days)
        {
            var pcReport = new PcReport
            {
                ComputerName = ComputerName,
                Days = Days
            };
            var addUser = AddUserAsync(pcReport);
            var addFileLogs = AddFileLogsAsync(pcReport);
            var addEventLogs = AddEventLogsAsync(pcReport);
            var addDatabaseLogs = AddDatabaseLogsAsync(pcReport);
            var tasks = new List<Task> { addUser, addFileLogs, addEventLogs, addDatabaseLogs };
            await Task.WhenAll(tasks);
            return pcReport;
        }


        private static async Task<PcReport> CreateReportConcurrentlyAsync(string ComputerName, int Days)
        {
            var getUser = GetUserAsync(ComputerName, Days);
            var getFileLogs = GetFileLogsAsync(ComputerName, Days);
            var getEventLogs = GetEventLogsAsync(ComputerName, Days);
            var getDatabaseLogs = GetDatabaseLogsAsync(ComputerName, Days);
            var tasks = new List<Task> {getUser, getFileLogs, getEventLogs, getDatabaseLogs};
            await Task.WhenAll(tasks);
            return new PcReport
            {
                PrimaryUser = getUser.Result,
                FileLogs = getFileLogs.Result,
                EventLogs = getEventLogs.Result,
                DatabaseLogs = getDatabaseLogs.Result
            };
        }

        
        private static async Task AddUserAsync(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding primary user of {PcReport.ComputerName}.", ConsoleColor.Cyan);
            await Task.Delay(TimeSpan.FromMilliseconds(_getUserMsec));
            PcReport.PrimaryUser = new User { Username = $"{PcReport.ComputerName} User" };
            ConsoleWriter.WriteLine(_stopwatch, $"The primary user is {PcReport.PrimaryUser.Username}.", ConsoleColor.Cyan);
        }


        private static async Task AddFileLogsAsync(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding file logs for {PcReport.ComputerName}.", ConsoleColor.Green);
            await Task.Delay(TimeSpan.FromMilliseconds(_getFileLogsMsecPerDay * PcReport.Days));
            if (PcReport.PrimaryUser != null) PcReport.FileLogs = new List<string> {"File Log 1", "File Log 2", "File Log 3"};
            ConsoleWriter.WriteLine(_stopwatch, $"File logs = {string.Join(", ", PcReport.FileLogs ?? new List<string>())}.", ConsoleColor.Green);
        }


        private static async Task AddEventLogsAsync(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding event logs for {PcReport.ComputerName}.", ConsoleColor.Magenta);
            await Task.Delay(TimeSpan.FromMilliseconds(_getEventLogsMsecPerDay * PcReport.Days));
            if (PcReport.PrimaryUser != null) PcReport.EventLogs = new List<string> { "Event Log 1", "Event Log 2", "Event Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"Event logs = {string.Join(", ", PcReport.EventLogs ?? new List<string>())}.", ConsoleColor.Magenta);
        }


        private static async Task AddDatabaseLogsAsync(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding database logs for {PcReport.ComputerName}.", ConsoleColor.Yellow);
            await Task.Delay(TimeSpan.FromMilliseconds(_getDatabaseLogsMsecPerDay * PcReport.Days));
            if (PcReport.PrimaryUser != null) PcReport.DatabaseLogs = new List<string> { "Database Log 1", "Database Log 2", "Database Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"Database logs = {string.Join(", ", PcReport.DatabaseLogs ?? new List<string>())}.", ConsoleColor.Yellow);
        }

        
        private static async Task<User> GetUserAsync(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding primary user of {ComputerName}.", ConsoleColor.Cyan);
            await Task.Delay(TimeSpan.FromMilliseconds(_getUserMsec * Days));
            var user = new User { Username = $"{ComputerName} User" };
            ConsoleWriter.WriteLine(_stopwatch, $"The primary user is {user.Username}.", ConsoleColor.Cyan);
            return user;
        }


        private static async Task<List<string>> GetFileLogsAsync(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding file logs for {ComputerName}.", ConsoleColor.Green);
            await Task.Delay(TimeSpan.FromMilliseconds(_getFileLogsMsecPerDay * Days));
            var fileLogs = new List<string> { "File Log 1", "File Log 2", "File Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"File logs = {string.Join(", ", fileLogs)}.", ConsoleColor.Green);
            return fileLogs;
        }


        private static async Task<List<string>> GetEventLogsAsync(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding event logs for {ComputerName}.", ConsoleColor.Magenta);
            await Task.Delay(TimeSpan.FromMilliseconds(_getEventLogsMsecPerDay * Days));
            var eventLogs = new List<string> { "Event Log 1", "Event Log 2", "Event Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"Event logs = {string.Join(", ", eventLogs)}.", ConsoleColor.Magenta);
            return eventLogs;
        }


        private static async Task<List<string>> GetDatabaseLogsAsync(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding database logs for {ComputerName}.", ConsoleColor.Yellow);
            await Task.Delay(TimeSpan.FromMilliseconds(_getDatabaseLogsMsecPerDay * Days));
            var databaseLogs = new List<string> { "Database Log 1", "Database Log 2", "Database Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"Database logs = {string.Join(", ", databaseLogs)}.", ConsoleColor.Yellow);
            return databaseLogs;
        }

        
        private static (string ComputerName, int Days, Func<string, int, Task<PcReport>> Technique) ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            var computerName = (Arguments.Count > 0) ? Arguments[0] : null;
            if (computerName.IsNullOrEmpty()) throw new ArgumentException("Specify a computer name.");
            var days = (Arguments.Count > 1) ? int.Parse(Arguments[1]) : 0;
            if (days == 0) throw new ArgumentException("Specify days.");
            var technique = (Arguments.Count > 2) ? Arguments[2].ToLower() : null;
            return technique switch
            {
                ("sequential") => (computerName, days, CreateReportSequentiallyAsync),
                ("concurrent-race") => (computerName, days, CreateReportConcurrentlyRaceConditionAsync),
                ("concurrent") => (computerName, days, CreateReportConcurrentlyAsync),
                _ => throw new ArgumentException(technique is null
                    ? "Specify a technique."
                    : $"{technique} not supported.")
            };
        }
    }
}
