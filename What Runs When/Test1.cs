using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace ErikTheCoder.Sandbox.WhatRunsWhen
{
    public static class Test1
    {
        private static Random _random;
        private static Stopwatch _stopwatch;


        public static async Task Run(Stopwatch Stopwatch)
        {
            _random = new Random();
            _stopwatch = Stopwatch;
            ConsoleWriter.WriteLine(_stopwatch, "Main: Running Test 1.");
            ConsoleWriter.WriteLine();
            ConsoleWriter.WriteLine(_stopwatch, "Main: Called CountWidgetsAsync.");
            ConsoleWriter.WriteLine(_stopwatch, "Main: Awaiting CountWidgetsAsync.");
            int widgets = await CountWidgetsAsync();
            ConsoleWriter.WriteLine(_stopwatch, $"Main: CountWidgetsAsync returned {widgets}.");
            ConsoleWriter.WriteLine(_stopwatch, "Main: Called CountSprocketsAsync.");
            ConsoleWriter.WriteLine(_stopwatch, "Main: Awaiting CountSprocketsAsync.");
            int sprockets = await CountSprocketsAsync();
            ConsoleWriter.WriteLine(_stopwatch, $"Main: CountSprocketsAsync returned {sprockets}.");
            int items = widgets + sprockets;
            ConsoleWriter.WriteLine(_stopwatch, $"Main: Found {items} items.");
        }


        private static async Task<int> CountWidgetsAsync()
        {
            ConsoleWriter.WriteLine(_stopwatch, "CountWidgetsAsync: Started.", ConsoleColor.Yellow);
            await Task.Delay(TimeSpan.FromSeconds(2));
            ConsoleWriter.WriteLine(_stopwatch, "CountWidgetsAsync: Finished.", ConsoleColor.Yellow);
            return _random.Next(1, 11);
        }


        private static async Task<int> CountSprocketsAsync()
        {
            ConsoleWriter.WriteLine(_stopwatch, "CountSprocketsAsync: Started.", ConsoleColor.Cyan);
            await Task.Delay(TimeSpan.FromSeconds(5));
            ConsoleWriter.WriteLine(_stopwatch, "CountSprocketsAsync: Finished.", ConsoleColor.Cyan);
            return _random.Next(1, 11);
        }
    }
}
