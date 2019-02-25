using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Refit;
using ErikTheCoder.Sandbox.Dapper.Contract;


namespace ErikTheCoder.Sandbox.Dapper.Client
{
    public static class Program
    {
        private const string _mappingServiceUrl = "http://localhost:55015";


        public static async Task Main()
        {
            IMappingService mappingService = RestService.For<IMappingService>(_mappingServiceUrl);
            Stopwatch stopwatch = Stopwatch.StartNew();
            GetOpenServiceCallsResponse response = await mappingService.GetOpenServiceCallsAsync();
            stopwatch.Stop();
            Console.WriteLine($"Retrieved {response.ServiceCalls.Count} service calls, {response.Customers.Count} customers, and {response.Technicians.Count} technicians in {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
        }
    }
}
