using System.Diagnostics;
using System.Threading.Tasks;


namespace ErikTheCoder.Sandbox.WhatRunsWhen
{
    public static class Program
    {
        public static async Task Main(string[] Args)
        {
            var stopwatch = Stopwatch.StartNew();
            ConsoleWriter.WriteLine();
            const int initialInput = 5;
            var test = Args.Length > 0 ? int.Parse(Args[0]) : 1;
            switch (test)
            {
                default:
                    await Test1.Run(stopwatch);
                    break;
                case 2:
                    await Test2.Run(stopwatch);
                    break;
                case 3:
                    Test3.Run(stopwatch, initialInput);
                    break;
                case 4:
                    await Test4.Run(stopwatch, initialInput);
                    break;
                case 5:
                    await Test5.Run(stopwatch, initialInput);
                    break;
            }
            ConsoleWriter.WriteLine();
            ConsoleWriter.WriteLine(stopwatch, $"Main: Test {test} complete.");
            ConsoleWriter.WriteLine();
        }
    }
}
