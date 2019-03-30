using System;
using System.Diagnostics;


namespace ErikTheCoder.Sandbox.AsyncConcurrent
{
    // This static class writes to the console in a thread-safe manner.
    public static class ConsoleWriter
    {
        private static readonly object _lock = new object();


        // ReSharper disable once UnusedMember.Global
        public static void WriteLine()
        {
            lock (_lock)
            {
                Console.WriteLine();
            }
        }


        public static void WriteLine(Stopwatch Stopwatch, string Message = null, ConsoleColor Color = ConsoleColor.White)
        {
            lock (_lock)
            {
                Console.ForegroundColor = Color;
                Console.WriteLine($"{Stopwatch.Elapsed.TotalSeconds:00.000}  Thread{System.Threading.Thread.CurrentThread.ManagedThreadId:00}  {Message}");
                Console.ResetColor();
            }
        }
    }
}
