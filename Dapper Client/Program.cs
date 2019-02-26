using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Refit;
using ErikTheCoder.Sandbox.Dapper.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;


namespace ErikTheCoder.Sandbox.Dapper.Client
{
    public static class Program
    {
        private const string _mappingServiceUrl = "http://localhost:5000";


        public static async Task Main()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                // Preserve case of property names.
                ContractResolver = new DefaultContractResolver(),
                // Preserve cyclical object references.
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            IMappingService mappingService = RestService.For<IMappingService>(_mappingServiceUrl);
            Stopwatch stopwatch = Stopwatch.StartNew();
            GetOpenServiceCallsResponse response = await mappingService.GetOpenServiceCallsAsync();
            stopwatch.Stop();
            Console.WriteLine($"Retrieved {response.ServiceCalls.Count} service calls, {response.Customers.Count} customers, and {response.Technicians.Count} technicians in {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
        }
    }
}
