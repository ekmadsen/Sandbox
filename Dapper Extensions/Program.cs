using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Dapper;
using ErikTheCoder.Data;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public delegate Task<(Dictionary<int, int> ServiceCallToTechnician, Dictionary<int, HashSet<int>> TechnicianToServiceCalls)> GetCallData(int MinServiceCallId, int MaxServiceCallId);


    public static class Program
    {
        private const string _connection = "Data Source=localhost;Initial Catalog=Dapper;Integrated Security=True";

        private const string _sql = @"
            select sc.Id as ServiceCallId, sc.TechnicianId
            from ServiceCalls sc
            where sc.Id between @MinServiceCallId and @MaxServiceCallId
            order by sc.Id asc";


        public static async Task Main(string[] Arguments)
        {
            var(getCallData, minServiceCallId, maxServiceCallId) = ParseCommandLine(Arguments);
            var stopWatch = Stopwatch.StartNew();
            var (serviceCallToTechnician, technicianToServiceCalls) = await getCallData(minServiceCallId, maxServiceCallId);
            stopWatch.Stop();
            Console.WriteLine($"Loaded {serviceCallToTechnician.Count} calls in {nameof(serviceCallToTechnician)} dictionary.");
            Console.WriteLine($"Loaded {technicianToServiceCalls.Count} service technicians in {nameof(technicianToServiceCalls)} dictionary.");
            Console.WriteLine($"Call data retrieved in {stopWatch.Elapsed.TotalSeconds:0.000} seconds.");
        }


        private static async Task<(Dictionary<int, int> ServiceCallToTechnician, Dictionary<int, HashSet<int>> TechnicianToServiceCalls)> GetCallDataRegularDapper(int MinServiceCallId, int MaxServiceCallId)
        {
            using (var connection = new SqlConnection(_connection))
            {
                await connection.OpenAsync();
                var serviceCallToTechnician = new Dictionary<int, int>();
                var technicianToServiceCalls = new Dictionary<int, HashSet<int>>();
                Func<int, int, int> map = (ServiceCallId, TechnicianId) =>
                {
                    serviceCallToTechnician.Add(ServiceCallId, TechnicianId);
                    if (!technicianToServiceCalls.ContainsKey(TechnicianId)) technicianToServiceCalls.Add(TechnicianId, new HashSet<int>());
                    technicianToServiceCalls[TechnicianId].Add(ServiceCallId);
                    return default;
                };
                var param = new {MinServiceCallId, MaxServiceCallId};
                // ReSharper disable once UnusedVariable
                var unusedReturnValues = await connection.QueryAsync(_sql, map, param, splitOn: "TechnicianId");
                return (serviceCallToTechnician, technicianToServiceCalls);
            }
        }


        private static async Task<(Dictionary<int, int> ServiceCallToTechnician, Dictionary<int, HashSet<int>> TechnicianToServiceCalls)> GetCallDataDapperExtension(int MinServiceCallId, int MaxServiceCallId)
        {
            using (var connection = new SqlConnection(_connection))
            {
                await connection.OpenAsync();
                var serviceCallToTechnician = new Dictionary<int, int>();
                var technicianToServiceCalls = new Dictionary<int, HashSet<int>>();
                Action<int, int> map = (ServiceCallId, TechnicianId) =>
                {
                    serviceCallToTechnician.Add(ServiceCallId, TechnicianId);
                    if (!technicianToServiceCalls.ContainsKey(TechnicianId)) technicianToServiceCalls.Add(TechnicianId, new HashSet<int>());
                    technicianToServiceCalls[TechnicianId].Add(ServiceCallId);
                };
                var param = new { MinServiceCallId, MaxServiceCallId };
                await connection.QueryAsync(_sql, map, param, SplitOn: "TechnicianId");
                return (serviceCallToTechnician, technicianToServiceCalls);
            }
        }


        private static (GetCallData GetCallData, int MinServiceCallId, int MaxServiceCallId) ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            const string errorMessage = "Specify a technique, min service call ID, and max service call ID.";
            if (Arguments == null || Arguments.Count != 3) throw new ArgumentException(errorMessage);
            var argumentValue = Arguments[0];
            GetCallData getCallData = argumentValue?.ToLower() switch
            {
                "regular" => GetCallDataRegularDapper,
                "extension" => GetCallDataDapperExtension,
                _ => throw new ArgumentException($"{argumentValue} technique not supported.")
            };
            var minServiceCallId = int.Parse(Arguments[1]);
            var maxServiceCallId = int.Parse(Arguments[2]);
            return (getCallData, minServiceCallId, maxServiceCallId);
        }
    }
}
