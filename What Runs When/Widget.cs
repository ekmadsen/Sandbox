using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace ErikTheCoder.Sandbox.WhatRunsWhen
{
    public class Widget
    {
        private readonly Stopwatch _stopwatch;
        private readonly int _complexity;
        public string Name { get; }
        
        
        public Widget(Stopwatch Stopwatch, string Name, int Complexity)
        {
            _stopwatch = Stopwatch;
            this.Name = Name;
            _complexity = Complexity;
        }


        public int Frob3(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Frob {Name}: Started.", ConsoleColor.Yellow);
            int delayMSec = Input * _complexity;
            Thread.Sleep(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Frob {Name}: Finished.", ConsoleColor.Yellow);
            return delayMSec;
        }


        public async Task<int> Frob4(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Frob {Name}: Started.", ConsoleColor.Yellow);
            int delayMSec = Input * _complexity;
            await Task.Delay(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Frob {Name}: Finished.", ConsoleColor.Yellow);
            return delayMSec;
        }


        public async Task<(string WidgetName, int FrobValue)> Frob5(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Frob {Name}: Started.", ConsoleColor.Yellow);
            int delayMSec = Input * _complexity;
            await Task.Delay(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Frob {Name}: Finished.", ConsoleColor.Yellow);
            return (Name, delayMSec);
        }


        public int Bork3(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Bork {Name}: Started.", ConsoleColor.Cyan);
            int delayMSec = Input * _complexity;
            Thread.Sleep(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Bork {Name}: Finished.", ConsoleColor.Cyan);
            return delayMSec;
        }


        public async Task<int> Bork4(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Bork {Name}: Started.", ConsoleColor.Cyan);
            int delayMSec = Input * _complexity;
            await Task.Delay(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Bork {Name}: Finished.", ConsoleColor.Cyan);
            return delayMSec;
        }


        public async Task<(string WidgetName, int BorkValue)> Bork5(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Bork {Name}: Started.", ConsoleColor.Cyan);
            int delayMSec = Input * _complexity;
            await Task.Delay(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Bork {Name}: Finished.", ConsoleColor.Cyan);
            return (Name, delayMSec);
        }


        public void Zap3(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Zap {Name}: Started.", ConsoleColor.Green);
            int delayMSec = Input * _complexity;
            Thread.Sleep(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Zap {Name}: Finished.", ConsoleColor.Green);
        }


        public async Task<int> Zap4(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Zap {Name}: Started.", ConsoleColor.Green);
            int delayMSec = Input * _complexity;
            await Task.Delay(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Zap {Name}: Finished.", ConsoleColor.Green);
            return delayMSec;
        }


        public async Task<(string WidgetName, int ZapValue)> Zap5(int Input)
        {
            ConsoleWriter.WriteLine(_stopwatch, $"Zap {Name}: Started.", ConsoleColor.Green);
            int delayMSec = Input * _complexity;
            await Task.Delay(TimeSpan.FromMilliseconds(delayMSec));
            ConsoleWriter.WriteLine(_stopwatch, $"Zap {Name}: Finished.", ConsoleColor.Green);
            return (Name, delayMSec);
        }
    }
}
