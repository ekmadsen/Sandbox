using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.ServiceProxy;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public static class Program
    {

        public static async Task Main(string[] Arguments)
        {
            try
            {
                Console.WriteLine();
                if (Arguments?.Length != 1) throw new ArgumentException("Version not specified.");
                await RunAsync(Arguments);
            }
            catch (Exception exception)
            {
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
            finally
            {
                Console.WriteLine();
            }
        }


        private static async Task RunAsync(IReadOnlyList<string> Arguments)
        {
            // Get given version of the async producer / consumer pipeline.
            int version = int.Parse(Arguments[0]);
            IPipeline pipeline = Pipeline.Create(version);
            // Get proxy to math service and configure initial values.
            Proxy<IMathService> mathService = GetProxy.For<IMathService>("http://localhost:65447");
            int[] inputValues = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            int[] stepValues = {0, 11, 12, 13};
            // Run pipeline.
            var stopwatch = Stopwatch.StartNew();
            await pipeline.Run(mathService, inputValues, stepValues);
            stopwatch.Stop();
            ThreadsafeConsole.WriteLine($"Pipeline ran in {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
        }
    }
}
