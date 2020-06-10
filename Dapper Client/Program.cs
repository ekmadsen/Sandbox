﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using ErikTheCoder.Sandbox.Dapper.Contract;
using ErikTheCoder.Utilities;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Dapper.Client
{
    public static class Program
    {
        private const string _mappingServiceUrlBase = "http://localhost:55015";
        private const string _mappingServiceUrl = _mappingServiceUrlBase + "/mapping/getopenservicecalls";
        private static HttpClient _httpClient;
        private static JsonSerializer _jsonSerializer;
        private static IMappingService _mappingService;


        public static async Task Main(string[] Arguments)
        {
            try
            {
                await Run(Arguments);
            }
            catch (Exception exception)
            {
                // This code is not thread safe.  But this program uses a single thread, so no issue.
                ConsoleColor restoreColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.GetSummary(true, true));
                Console.ForegroundColor = restoreColor;
            }
        }


        private static async Task Run(IReadOnlyList<string> Arguments)
        {
            // Configure Json.NET and Refit.
            _httpClient = new HttpClient(new CacheBustingMessageHandler()) { BaseAddress = new Uri(_mappingServiceUrlBase) };
            _jsonSerializer = new JsonSerializer();
            _mappingService = RestService.For<IMappingService>(_httpClient);
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                // Preserve case of property names.
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver(),
                // Preserve cyclical object references.
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            Func<Task<GetOpenServiceCallsResponse>> getOpenServiceCalls = ParseCommandLine(Arguments);
            do
            {
                // Call web service using specified function, then verify object references are preserved.
                Stopwatch stopwatch = Stopwatch.StartNew();
                GetOpenServiceCallsResponse response = await getOpenServiceCalls();
                stopwatch.Stop();
                Console.WriteLine($"Retrieved {response.ServiceCalls.Count} service calls, {response.Customers.Count} customers, and {response.Technicians.Count} technicians in {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
                VerifyObjectReferencesPreserved(response);
            } while (string.IsNullOrWhiteSpace(Console.ReadLine()));
        }


        private static Func<Task<GetOpenServiceCallsResponse>> ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            // Parse deserializer.
            Func<Task<GetOpenServiceCallsResponse>> getOpenServiceCalls;
            string deserializer = (Arguments.Count > 0) ? Arguments[0].ToLower() : null;
            switch (deserializer)
            {
                case "json.net":
                    getOpenServiceCalls = GetOpenServiceCallsViaJsonNet;
                    break;
                case "refit":
                    getOpenServiceCalls = GetOpenServiceCallsViaRefit;
                    break;
                default:
                    throw new ArgumentException(deserializer is null ? "Specify a deserialzer." : $"{deserializer} not supported.");
            }
            return getOpenServiceCalls;
        }


        private static async Task<GetOpenServiceCallsResponse> GetOpenServiceCallsViaJsonNet()
        {
            using (HttpResponseMessage responseMessage = await _httpClient.GetAsync(new Uri(_mappingServiceUrl)))
            {
                using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                        {
                            return _jsonSerializer.Deserialize<GetOpenServiceCallsResponse>(jsonReader);
                        }
                    }
                }
            }
        }


        private static async Task<GetOpenServiceCallsResponse> GetOpenServiceCallsViaRefit() => await _mappingService.GetOpenServiceCallsAsync();


        private static void VerifyObjectReferencesPreserved(GetOpenServiceCallsResponse Response)
        {
            const int technicianId = 3276;
            const int customerId = 75904;
            const int serviceCallId = 8949862;
            Technician technician = Response.Technicians[technicianId];
            Customer customer = Response.Customers[customerId];
            ServiceCall serviceCall = Response.ServiceCalls[serviceCallId];
            Trace.Assert(ReferenceEquals(technician, customer.Technicians[technicianId]));
            Trace.Assert(ReferenceEquals(technician, serviceCall.Technician));
            Trace.Assert(ReferenceEquals(customer, technician.Customers[customerId]));
            Trace.Assert(ReferenceEquals(customer, serviceCall.Customer));
            Trace.Assert(ReferenceEquals(serviceCall, technician.ServiceCalls[serviceCallId]));
            Trace.Assert(ReferenceEquals(serviceCall, customer.ServiceCalls[serviceCallId]));
        }
    }
}
