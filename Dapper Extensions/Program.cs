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
            (GetCallData getCallData, int minServiceCallId, int maxServiceCallId) = ParseCommandLine(Arguments);
            (Dictionary<int, int> serviceCallToTechnician, Dictionary<int, HashSet<int>> technicianToServiceCalls) = await getCallData(minServiceCallId, maxServiceCallId);
            Stopwatch stopWatch = Stopwatch.StartNew();
            Console.WriteLine($"Loaded {serviceCallToTechnician.Count} calls in {nameof(serviceCallToTechnician)} dictionary.");
            Console.WriteLine($"Loaded {technicianToServiceCalls.Count} service technicians in {nameof(technicianToServiceCalls)} dictionary.");
            stopWatch.Stop();
            Console.WriteLine($"Call data retrieved in {stopWatch.Elapsed.TotalSeconds:0.000} seconds.");
        }


        private static async Task<(Dictionary<int, int> ServiceCallToTechnician, Dictionary<int, HashSet<int>> TechnicianToServiceCalls)> GetCallDataRegularDapper(int MinServiceCallId, int MaxServiceCallId)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                await connection.OpenAsync();
                Dictionary<int, int> serviceCallToTechnician = new Dictionary<int, int>();
                Dictionary<int, HashSet<int>> technicianToServiceCalls = new Dictionary<int, HashSet<int>>();
                Func<int, int, int> map = (ServiceCallId, TechnicianId) =>
                {
                    serviceCallToTechnician.Add(ServiceCallId, TechnicianId);
                    if (!technicianToServiceCalls.ContainsKey(TechnicianId)) technicianToServiceCalls.Add(TechnicianId, new HashSet<int>());
                    technicianToServiceCalls[TechnicianId].Add(ServiceCallId);
                    return default;
                };
                var param = new {MinServiceCallId, MaxServiceCallId};
                IEnumerable<int> unusedReturnValues = await connection.QueryAsync(_sql, map, param, splitOn: "TechnicianId");
                return (serviceCallToTechnician, technicianToServiceCalls);
            }
        }


        private static async Task<(Dictionary<int, int> ServiceCallToTechnician, Dictionary<int, HashSet<int>> TechnicianToServiceCalls)> GetCallDataDapperExtension(int MinServiceCallId, int MaxServiceCallId)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                await connection.OpenAsync();
                Dictionary<int, int> serviceCallToTechnician = new Dictionary<int, int>();
                Dictionary<int, HashSet<int>> technicianToServiceCalls = new Dictionary<int, HashSet<int>>();
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
            GetCallData getCallData;
            string argumentValue = Arguments[0];
            switch (argumentValue?.ToLower())
            {
                case "regular":
                    getCallData = GetCallDataRegularDapper;
                    break;
                case "extension":
                    getCallData = GetCallDataDapperExtension;
                    break;
                default:
                    throw new ArgumentException($"{argumentValue} technique not supported.");
            }
            int minServiceCallId = int.Parse(Arguments[1]);
            int maxServiceCallId = int.Parse(Arguments[2]);
            return (getCallData, minServiceCallId, maxServiceCallId);
        }
    }
}
