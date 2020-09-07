using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.Utilities;
using Refit;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public static class Program
    {
        private const int _inputValueCount = 100;


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
            var version = int.Parse(Arguments[0]);
            var pipeline = Pipeline.Create(version);
            // Get proxy to math service and configure initial values.
            var httpClient = new HttpClient(new CacheBustingMessageHandler()) { BaseAddress = new Uri("http://localhost:65447") };
            var mathService = RestService.For<IMathService>(httpClient);
            var inputValues = new long[_inputValueCount];
            for (var index = 0; index < _inputValueCount; index++) inputValues[index] = index + 1;
            var stepValues = new long[]{4, 99, 13, 41};
            // Run pipeline.
            var stopwatch = Stopwatch.StartNew();
            await pipeline.Run(mathService, inputValues, stepValues);
            stopwatch.Stop();
            ThreadsafeConsole.WriteLine($"Pipeline ran in {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
        }
    }
}
