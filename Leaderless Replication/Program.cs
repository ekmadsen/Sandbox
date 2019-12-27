using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public static class Program
    {
        private const string _sentinelKey = "Sentinel Key";
        private const string _timeSpanFormat = "00.000";
        private static readonly IThreadsafeRandom _random = new ThreadsafeRandom();
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();


        public static async Task Main(string[] Arguments)
        {
            try
            {
                ThreadsafeConsole.WriteLine();
                if (Arguments?.Length != 1) throw new ArgumentException("Test name not specified.");
                await Run(Arguments);
            }
            catch (Exception exception)
            {
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
            finally
            {
                ThreadsafeConsole.WriteLine();
            }
        }


        private static async Task Run(IReadOnlyList<string> Arguments)
        {
            // Create nodes in all regions and connect them.
            List<NodeBase> globalNodes = new List<NodeBase>();
            int id = 0;
            List<NodeBase> naNodes = new List<NodeBase>();
            CreateNodes(globalNodes, naNodes, RegionName.NorthAmerica, ref id);
            List<NodeBase> saNodes = new List<NodeBase>();
            CreateNodes(globalNodes, saNodes, RegionName.SouthAmerica, ref id);
            List<NodeBase> euNodes = new List<NodeBase>();
            CreateNodes(globalNodes, euNodes, RegionName.Europe, ref id);
            List<NodeBase> asNodes = new List<NodeBase>();
            CreateNodes(globalNodes, asNodes, RegionName.Asia, ref id);
            foreach (NodeBase node in globalNodes) node.Connect(globalNodes);
            // Create regions.
            Region naRegion = new Region(RegionName.NorthAmerica, naNodes);
            Region saRegion = new Region(RegionName.SouthAmerica, saNodes);
            Region euRegion = new Region(RegionName.Europe, euNodes);
            Region asRegion = new Region(RegionName.Asia, asNodes);
            // Create clients and connect them to their regional nodes.
            Client naClient = new Client(_random, id, "NA-Client", RegionName.NorthAmerica);
            id++;
            Client saClient = new Client(_random, id, "SA-Client", RegionName.SouthAmerica);
            id++;
            Client euClient = new Client(_random, id, "EU-Client", RegionName.Europe);
            id++;
            Client asClient = new Client(_random, id, "AS-Client", RegionName.Asia);
            (TimeSpan MinLatency, TimeSpan MaxLatency) regionalLatency = (TimeSpan.FromMilliseconds(Latency.Min), TimeSpan.FromMilliseconds(Latency.Max));
            naClient.Connect(naNodes, regionalLatency);
            saClient.Connect(saNodes, regionalLatency);
            euClient.Connect(euNodes, regionalLatency);
            asClient.Connect(asNodes, regionalLatency);
            // Run test.
            string testName = Arguments[0];
            switch (testName?.ToLower())
            {
                case "testreadperformance":
                    await TestReadPerformance(naClient, naRegion, asRegion);
                    break;
                case "testfaulttolerance":
                    await TestFaultTolerance(naClient, naRegion);
                    break;
                default:
                    throw new ArgumentException($"{testName} test not supported.");
            }
        }


        private static void CreateNodes(ICollection<NodeBase> GlobalNodes, ICollection<NodeBase> RegionalNodes,  string RegionName, ref int Id)
        {
            const int nodesPerRegion = 5;
            const int requiredVotes = 3;
            while (RegionalNodes.Count < nodesPerRegion)
            {
                QuorumNode node = new QuorumNode(_random, Id, $"NA-{RegionalNodes.Count + 1}", RegionName, requiredVotes);
                GlobalNodes.Add(node);
                RegionalNodes.Add(node);
                Id++;
            }
        }


        // ReSharper disable once SuggestBaseTypeForParameter
        private static async Task TestReadPerformance(Client NaClient, Region NaRegion, Region AsRegion)
        {
            // Test client performance.
            ThreadsafeConsole.WriteLine("Testing performance of North American client.");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.Write("Writing sentinel value... ");
            TimeSpan begin = _stopwatch.Elapsed;
            await NaClient.WriteValueAsync(_sentinelKey, "Yada, yada, yada");
            TimeSpan end = _stopwatch.Elapsed;
            TimeSpan clientWriteDuration = end - begin;
            ThreadsafeConsole.WriteLine("done.");
            ThreadsafeConsole.WriteLine($"Wrote sentinel value in {clientWriteDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            string sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            TimeSpan clientReadDuration = end - begin;
            ThreadsafeConsole.WriteLine("done.");
            ThreadsafeConsole.WriteLine($"Sentinel value = {sentinelValue}.");
            ThreadsafeConsole.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            ThreadsafeConsole.WriteLine();
            // Compare client read performance to reading from a high-latency connection.
            ThreadsafeConsole.WriteLine("Testing performance of connection between Asian node and North American node.");
            ThreadsafeConsole.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            Connection asToNaConnection = AsRegion.Nodes[0].Connections[RegionName.NorthAmerica][0];
            sentinelValue = await asToNaConnection.GetValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            TimeSpan highLatencyDuration = end - begin;
            ThreadsafeConsole.WriteLine("done.");
            ThreadsafeConsole.WriteLine($"Sentinel value = {sentinelValue}.");
            ThreadsafeConsole.WriteLine($"Read sentinel value in {highLatencyDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.Write("Test result: ");
            if (clientReadDuration < highLatencyDuration) ThreadsafeConsole.WriteLine("success.", ConsoleColor.Green);
            else ThreadsafeConsole.WriteLine("failure.", ConsoleColor.Red);
        }


        // ReSharper disable once SuggestBaseTypeForParameter
        private static async Task TestFaultTolerance(Client NaClient, Region NaRegion)
        {
            ThreadsafeConsole.WriteLine("Testing fault tolerance of North American nodes.");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.Write("Writing sentinel value... ");
            TimeSpan begin = _stopwatch.Elapsed;
            await NaClient.WriteValueAsync(_sentinelKey, "Yada, yada, yada");
            TimeSpan end = _stopwatch.Elapsed;
            TimeSpan clientWriteDuration = end - begin;
            ThreadsafeConsole.WriteLine("done.");
            ThreadsafeConsole.WriteLine($"Wrote sentinel value in {clientWriteDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            ThreadsafeConsole.WriteLine();
            // Take two nodes offline.  Expect no service interruption.
            ThreadsafeConsole.Write("Taking two (of five) nodes offline... ");
            NaRegion.Nodes[1].Online = false;
            NaRegion.Nodes[2].Online = false;
            ThreadsafeConsole.WriteLine("done.");
            ThreadsafeConsole.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            string sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            TimeSpan clientReadDuration = end - begin;
            ThreadsafeConsole.WriteLine("done.");
            ThreadsafeConsole.WriteLine($"Sentinel value = {sentinelValue}.");
            ThreadsafeConsole.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            ThreadsafeConsole.WriteLine();
            // Take a third node offline.  Expect service interruption.
            ThreadsafeConsole.Write("Taking a third (of five) node offline... ");
            NaRegion.Nodes[4].Online = false;
            ThreadsafeConsole.WriteLine("done.");
            ThreadsafeConsole.Write("Reading sentinel value... ");
            bool success = false;
            try
            {
                begin = _stopwatch.Elapsed;
                sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
                end = _stopwatch.Elapsed;
                clientReadDuration = end - begin;
                ThreadsafeConsole.WriteLine("done.");
                ThreadsafeConsole.WriteLine($"Sentinel value = {sentinelValue}.");
                ThreadsafeConsole.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
                ThreadsafeConsole.WriteLine();
            }
            catch (QuorumNotReachedException exception)
            {
                // Expect a QuorumNotReachedException because only two of five nodes are online.
                success = true;
                ThreadsafeConsole.WriteLine();
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
            catch (Exception exception)
            {
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }

            ThreadsafeConsole.Write("Test result: ");
            if (success) ThreadsafeConsole.WriteLine("success.", ConsoleColor.Green);
            else ThreadsafeConsole.WriteLine("failure.", ConsoleColor.Red);
        }


        private static async Task TestEventualConsistency()
        {
            // Demonstrate read repair.
            await Task.Delay(0);
        }
    }
}
