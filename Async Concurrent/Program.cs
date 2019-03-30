using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ErikTheCoder.Logging;
using ErikTheCoder.ServiceContract;


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
            (string pcName, int days, Func<string, int, Task<PcReport>> createReport) = ParseCommandLine(Arguments);
            // ReSharper disable once UnusedVariable
            PcReport pcReport = await createReport(pcName, days);
        }


        private static async Task<PcReport> CreateReportSequentiallyAsync(string ComputerName, int Days)
        {
            PcReport pcReport = new PcReport
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
            PcReport pcReport = new PcReport
            {
                ComputerName = ComputerName,
                Days = Days
            };
            Task addUser = AddUserAsync(pcReport);
            Task addFileLogs = AddFileLogsAsync(pcReport);
            Task addEventLogs = AddEventLogsAsync(pcReport);
            Task addDatabaseLogs = AddDatabaseLogsAsync(pcReport);
            List<Task> tasks = new List<Task> { addUser, addFileLogs, addEventLogs, addDatabaseLogs };
            await Task.WhenAll(tasks);
            return pcReport;
        }


        private static async Task<PcReport> CreateReportConcurrentlyAsync(string ComputerName, int Days)
        {
            Task<User> getUser = GetUserAsync(ComputerName, Days);
            Task<List<string>> getFileLogs = GetFileLogsAsync(ComputerName, Days);
            Task<List<string>> getEventLogs = GetEventLogsAsync(ComputerName, Days);
            Task<List<string>> getDatabaseLogs = GetDatabaseLogsAsync(ComputerName, Days);
            List<Task> tasks = new List<Task> {getUser, getFileLogs, getEventLogs, getDatabaseLogs};
            await Task.WhenAll(tasks);
            return new PcReport
            {
                PrimaryUser = getUser.Result,
                FileLogs = getFileLogs.Result,
                EventLogs = getEventLogs.Result
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
            User user = new User { Username = $"{ComputerName} User" };
            ConsoleWriter.WriteLine(_stopwatch, $"The primary user is {user.Username}.", ConsoleColor.Cyan);
            return user;
        }


        private static async Task<List<string>> GetFileLogsAsync(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding file logs for {ComputerName}.", ConsoleColor.Green);
            await Task.Delay(TimeSpan.FromMilliseconds(_getFileLogsMsecPerDay * Days));
            List<string> fileLogs = new List<string> { "File Log 1", "File Log 2", "File Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"File logs = {string.Join(", ", fileLogs)}.", ConsoleColor.Green);
            return fileLogs;
        }


        private static async Task<List<string>> GetEventLogsAsync(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding event logs for {ComputerName}.", ConsoleColor.Magenta);
            await Task.Delay(TimeSpan.FromMilliseconds(_getEventLogsMsecPerDay * Days));
            List<string> eventLogs = new List<string> { "Event Log 1", "Event Log 2", "Event Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"Event logs = {string.Join(", ", eventLogs)}.", ConsoleColor.Magenta);
            return eventLogs;
        }


        private static async Task<List<string>> GetDatabaseLogsAsync(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding database logs for {ComputerName}.", ConsoleColor.Yellow);
            await Task.Delay(TimeSpan.FromMilliseconds(_getDatabaseLogsMsecPerDay * Days));
            List<string> databaseLogs = new List<string> { "Database Log 1", "Database Log 2", "Database Log 3" };
            ConsoleWriter.WriteLine(_stopwatch, $"Database logs = {string.Join(", ", databaseLogs)}.", ConsoleColor.Yellow);
            return databaseLogs;
        }

        
        private static (string ComputerName, int Days, Func<string, int, Task<PcReport>> Technique) ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            string computerName = (Arguments.Count > 0) ? Arguments[0] : null;
            int days = (Arguments.Count > 1) ? int.Parse(Arguments[1]) : 0;
            if (days == 0) throw new ArgumentException("Specify days.");
            if (string.IsNullOrEmpty(computerName)) throw new ArgumentException("Specify a computer name.");
            string technique = (Arguments.Count > 2) ? Arguments[2].ToLower() : null;
            switch (technique)
            {
                case ("sequential"):
                    return (computerName, days, CreateReportSequentiallyAsync);
                case ("concurrent-race"):
                    return (computerName, days, CreateReportConcurrentlyRaceConditionAsync);
                case ("concurrent"):
                    return (computerName, days, CreateReportConcurrentlyAsync);
                default:
                    throw new ArgumentException(technique is null ? "Specify a technique." : $"{technique} not supported.");
            }
        }
    }
}
