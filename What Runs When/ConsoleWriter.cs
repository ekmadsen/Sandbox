using System;
using System.Diagnostics;


namespace ErikTheCoder.Sandbox.WhatRunsWhen
{
    public static class ConsoleWriter
    {
        private static readonly object _syncLock = new object();


        public static void WriteLine()
        {
            lock (_syncLock)
            {
                Console.WriteLine();
            }
        }


        public static void WriteLine(Stopwatch Stopwatch, string Message = null, ConsoleColor Color = ConsoleColor.White)
        {
            // Write to console in a thread-safe manner.
            lock (_syncLock)
            {
                Console.ForegroundColor = Color;
                Console.WriteLine($"{Stopwatch.Elapsed.TotalSeconds:00.000}  Thread{System.Threading.Thread.CurrentThread.ManagedThreadId:00}  {Message}");
                Console.ResetColor();
            }
        }
    }
}
