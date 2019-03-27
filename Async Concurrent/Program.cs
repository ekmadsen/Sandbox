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
        private static int _getUserMsec = 50;
        private static int _getFileLogsMsecPerDay = 150;
        private static int _getEventLogsMsecPerDay = 75;
        private static int _getDatabaseLogsMsecPerDay = 100;
        private static int _getInternetLogsMsecPerDay = 200;
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();


        public static async Task Main(string[] Arguments)
        {
            try
            {
                await Run(Arguments);
            }
            catch (Exception exception)
            {
                ConsoleWriter.WriteLine(_stopwatch, exception.GetSummary(true, true), ConsoleColor.Red);
            }
        }


        private static async Task Run(IReadOnlyList<string> Arguments)
        {
            (string pcName, int days, Func<string, int, Task<PcReport>> createReport) = ParseCommandLine(Arguments);
            PcReport pcReport = await createReport(pcName, days);
        }


        private static async Task<PcReport> CreateReportSequentially(string ComputerName, int Days)
        {
            PcReport pcReport = new PcReport
            {
                ComputerName = ComputerName,
                Days = Days
            };
            await AddUser(pcReport);
            await AddFileLogs(pcReport);
            await AddEventLogs(pcReport);
            return pcReport;
        }


        private static async Task<PcReport> CreateReportConcurrentlyRaceCondition(string ComputerName, int Days)
        {
            return await Task.FromResult(new PcReport());
        }


        private static async Task<PcReport> CreateReportConcurrently(string ComputerName, int Days)
        {
            Task<User> getUser = GetUser(ComputerName, Days);
            Task<List<string>> getFileLogs = GetFileLogs(ComputerName, Days);
            Task<List<string>> getEventLogs = GetEventLogs(ComputerName, Days);
            List<Task> tasks = new List<Task>() {getUser, getFileLogs, getEventLogs};
            await Task.WhenAll(tasks);
            return new PcReport
            {
                PrimaryUser = getUser.Result,
                FileLogs = getFileLogs.Result,
                EventLogs = getEventLogs.Result
            };
        }



        // Add Methods
        private static async Task AddUser(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, "Adding user.", ConsoleColor.Cyan);
            await Task.Delay(TimeSpan.FromMilliseconds(_getUserMsec));
            PcReport.PrimaryUser = new User { Username = $"{PcReport.ComputerName} User" };
            ConsoleWriter.WriteLine(_stopwatch, "Added user.", ConsoleColor.Cyan);
        }


        private static async Task AddFileLogs(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, "Adding file logs.", ConsoleColor.Green);
            if (PcReport.PrimaryUser != null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(_getFileLogsMsecPerDay * PcReport.Days));
                PcReport.FileLogs = new List<string> {"File Log 1", "File Log 2", "File Log 3"};
            }
            ConsoleWriter.WriteLine(_stopwatch, "Added file logs.", ConsoleColor.Green);
        }


        private static async Task AddEventLogs(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, "Adding event logs.", ConsoleColor.Magenta);
            if (PcReport.PrimaryUser != null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(_getEventLogsMsecPerDay * PcReport.Days));
                PcReport.EventLogs = new List<string> { "Event Log 1", "Event Log 2", "Event Log 3" };
            }
            ConsoleWriter.WriteLine(_stopwatch, "Added event logs.", ConsoleColor.Magenta);
        }


        private static async Task AddDatabaseLogs(PcReport PcReport)
        {
            ConsoleWriter.WriteLine(_stopwatch, "Adding database logs.", ConsoleColor.Yellow);
            if (PcReport.PrimaryUser != null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(_getDatabaseLogsMsecPerDay * PcReport.Days));
                PcReport.EventLogs = new List<string> { "Event Log 1", "Event Log 2", "Event Log 3" };
            }
            ConsoleWriter.WriteLine(_stopwatch, "Added database logs.", ConsoleColor.Yellow);
        }


        // Get Methods
        private static async Task<User> GetUser(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding primary user of {ComputerName}.", ConsoleColor.Cyan);
            await Task.Delay(TimeSpan.FromMilliseconds(_getUserMsec * Days));
            ConsoleWriter.WriteLine(_stopwatch, $"Added primary user of {ComputerName}.", ConsoleColor.Cyan);
            return new User { Username = $"{ComputerName} User" };
        }


        private static async Task<List<string>> GetFileLogs(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding file logs for {ComputerName}.", ConsoleColor.Green);
            await Task.Delay(TimeSpan.FromMilliseconds(_getFileLogsMsecPerDay * Days));
            ConsoleWriter.WriteLine(_stopwatch, $"Added file logs for {ComputerName}.", ConsoleColor.Green);
            return new List<string> { "File Log 1", "File Log 2", "File Log 3" };
        }


        private static async Task<List<string>> GetEventLogs(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding event logs for {ComputerName}.", ConsoleColor.Magenta);
            await Task.Delay(TimeSpan.FromMilliseconds(_getEventLogsMsecPerDay * Days));
            ConsoleWriter.WriteLine(_stopwatch, $"Added event logs for {ComputerName}.", ConsoleColor.Magenta);
            return new List<string> { "Event Log 1", "Event Log 2", "Event Log 3" };
        }


        private static async Task<List<string>> GetDatabaseLogs(string ComputerName, int Days)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Adding database logs for {ComputerName}.", ConsoleColor.Yellow);
            await Task.Delay(TimeSpan.FromMilliseconds(_getDatabaseLogsMsecPerDay * Days));
            ConsoleWriter.WriteLine(_stopwatch, $"Added database logs for {ComputerName}.", ConsoleColor.Yellow);
            return new List<string> { "File Log 1", "File Log 2", "File Log 3" };
        }

        
        private static (string ComputerName, int Days, Func<string, int, Task<PcReport>> Technique) ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            string computerName = (Arguments.Count > 0) ? Arguments[0].ToLower() : null;
            int days = (Arguments.Count > 1) ? int.Parse(Arguments[1]) : 0;
            if (days == 0) throw new ArgumentException("Specify days.");
            if (string.IsNullOrEmpty(computerName)) throw new ArgumentException("Specify a computer name.");
            string technique = (Arguments.Count > 2) ? Arguments[2].ToLower() : null;
            switch (technique)
            {
                case ("sequential"):
                    return (computerName, days, CreateReportSequentially);
                case ("concurrent-race"):
                    return (computerName, days, CreateReportConcurrentlyRaceCondition);
                case ("concurrent"):
                    return (computerName, days, CreateReportConcurrently);
                default:
                    throw new ArgumentException(technique is null ? "Specify a technique." : $"{technique} not supported.");
            }
        }
    }
}
