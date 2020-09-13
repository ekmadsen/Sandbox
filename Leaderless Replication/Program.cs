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
        private const string _timeSpanFormat = "0.000";
        private static readonly IThreadsafeRandom _random = new ThreadsafeRandom();
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();


        public static async Task Main(string[] Arguments)
        {
            try
            {
                Console.WriteLine();
                if (Arguments?.Length != 1) throw new ArgumentException("Test name not specified.");
                await Run(Arguments);
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


        private static async Task Run(IReadOnlyList<string> Arguments)
        {
            // Create nodes in all regions and connect them.
            var globalNodes = new List<NodeBase>();
            var id = -1;
            var naNodes = new List<NodeBase>();
            CreateNodes(globalNodes, naNodes, RegionName.NorthAmerica, ref id);
            var saNodes = new List<NodeBase>();
            CreateNodes(globalNodes, saNodes, RegionName.SouthAmerica, ref id);
            var euNodes = new List<NodeBase>();
            CreateNodes(globalNodes, euNodes, RegionName.Europe, ref id);
            var asNodes = new List<NodeBase>();
            CreateNodes(globalNodes, asNodes, RegionName.Asia, ref id);
            foreach (var node in globalNodes) node.Connect(globalNodes);
            // Create regions.
            var naRegion = new Region(RegionName.NorthAmerica, naNodes);
            // ReSharper disable UnusedVariable
            var saRegion = new Region(RegionName.SouthAmerica, saNodes);
            var euRegion = new Region(RegionName.Europe, euNodes);
            var asRegion = new Region(RegionName.Asia, asNodes);
            // ReSharper restore UnusedVariable
            // Create clients and connect them to their regional nodes.
            var naClient = new Client(_random, ++id, "NA-Client", RegionName.NorthAmerica);
            var saClient = new Client(_random, ++id, "SA-Client", RegionName.SouthAmerica);
            var euClient = new Client(_random, ++id, "EU-Client", RegionName.Europe);
            var asClient = new Client(_random, ++id, "AS-Client", RegionName.Asia);
            var regionalLatency = (TimeSpan.FromMilliseconds(Latency.Min), TimeSpan.FromMilliseconds(Latency.Max));
            naClient.Connect(naNodes, regionalLatency);
            saClient.Connect(saNodes, regionalLatency);
            euClient.Connect(euNodes, regionalLatency);
            asClient.Connect(asNodes, regionalLatency);
            // Run test.
            var testName = Arguments[0];
            switch (testName?.ToLower())
            {
                case "testreadperformance":
                    await TestReadPerformance(naClient, asRegion);
                    break;
                case "testfaulttolerance":
                    await TestFaultTolerance(naClient, naRegion);
                    break;
                case "testeventualconsistency":
                    await TestEventualConsistency(naClient, naRegion, globalNodes);
                    break;
                default:
                    throw new ArgumentException($"{testName} test not supported.");
            }
        }


        private static void CreateNodes(ICollection<NodeBase> GlobalNodes, ICollection<NodeBase> RegionalNodes, string RegionName, ref int Id)
        {
            const int nodesPerRegion = 5;
            const int requiredVotes = 3;
            while (RegionalNodes.Count < nodesPerRegion)
            {
                var node = new QuorumNode(_random, ++Id, $"{RegionName}-{RegionalNodes.Count + 1}", RegionName, requiredVotes, true);
                GlobalNodes.Add(node);
                RegionalNodes.Add(node);
            }
        }


        // ReSharper disable once SuggestBaseTypeForParameter
        private static async Task TestReadPerformance(Client NaClient, Region AsRegion)
        {
            // Test client performance.
            Console.WriteLine("Testing performance of North American client.");
            Console.WriteLine();
            Console.Write("Writing sentinel value... ");
            var begin = _stopwatch.Elapsed;
            await NaClient.WriteValueAsync(_sentinelKey, "Yada, yada, yada");
            var end = _stopwatch.Elapsed;
            var clientWriteDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Wrote sentinel value in {clientWriteDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            Console.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            var sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            var clientReadDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Sentinel value = {sentinelValue}.");
            Console.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            // Compare client read performance to reading from a high-latency connection.
            Console.WriteLine("Testing performance of connection between Asian node and North American node.");
            Console.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            var asToNaConnection = AsRegion.Nodes[0].Connections[RegionName.NorthAmerica][0];
            sentinelValue = await asToNaConnection.GetValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            var highLatencyDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Sentinel value = {sentinelValue}.");
            Console.WriteLine($"Read sentinel value in {highLatencyDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            Console.Write("Test result: ");
            if (clientReadDuration < highLatencyDuration) ThreadsafeConsole.WriteLine("success.", ConsoleColor.Green);
            else ThreadsafeConsole.WriteLine("failure.", ConsoleColor.Red);
        }


        // ReSharper disable once SuggestBaseTypeForParameter
        private static async Task TestFaultTolerance(Client NaClient, Region NaRegion)
        {
            Console.WriteLine("Testing fault tolerance of North American nodes.");
            Console.WriteLine();
            Console.Write("Writing sentinel value... ");
            var begin = _stopwatch.Elapsed;
            await NaClient.WriteValueAsync(_sentinelKey, "Yada, yada, yada");
            var end = _stopwatch.Elapsed;
            var clientWriteDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Wrote sentinel value in {clientWriteDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            Console.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            var sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            var clientReadDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Sentinel value = {sentinelValue}.");
            Console.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            // Take two nodes offline.  Expect no service interruption.
            Console.Write("Taking two (of five) nodes offline... ");
            NaRegion.Nodes[2].Online = false;
            NaRegion.Nodes[3].Online = false;
            Console.WriteLine("done.");
            Console.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            clientReadDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Sentinel value = {sentinelValue}.");
            Console.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            // Take a third node offline.  Expect service interruption.
            Console.Write("Taking a third (of five) node offline... ");
            NaRegion.Nodes[4].Online = false;
            Console.WriteLine("done.");
            Console.Write("Reading sentinel value... ");
            var success = false;
            try
            {
                begin = _stopwatch.Elapsed;
                sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
                end = _stopwatch.Elapsed;
                clientReadDuration = end - begin;
                Console.WriteLine("done.");
                Console.WriteLine($"Sentinel value = {sentinelValue}.");
                Console.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
                Console.WriteLine();
            }
            catch (QuorumNotReachedException exception)
            {
                // Expect a QuorumNotReachedException because only two of five nodes are online.
                success = true;
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
            catch (Exception exception)
            {
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
            Console.Write("Test result: ");
            if (success) ThreadsafeConsole.WriteLine("success.", ConsoleColor.Green);
            else ThreadsafeConsole.WriteLine("failure.", ConsoleColor.Red);
        }

        
        // ReSharper disable once SuggestBaseTypeForParameter
        private static async Task TestEventualConsistency(Client NaClient, Region NaRegion, IReadOnlyCollection<NodeBase> GlobalNodes)
        {
            Console.WriteLine("Testing eventual consistency of global nodes.");
            Console.WriteLine();
            Console.Write("Writing sentinel value... ");
            var begin = _stopwatch.Elapsed;
            await NaClient.WriteValueAsync(_sentinelKey, "Before");
            var end = _stopwatch.Elapsed;
            var clientWriteDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Wrote sentinel value in {clientWriteDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            // Wait for writes to complete globally.
            Console.Write("Waiting for write to complete globally... ");
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine("done.");
            Console.WriteLine();
            Console.Write("Reading sentinel value... ");
            begin = _stopwatch.Elapsed;
            var sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            var clientReadDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Sentinel value = {sentinelValue}.");
            Console.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            // Take two regional nodes offline.
            Console.Write("Taking two (of five) nodes offline... ");
            NaRegion.Nodes[2].Online = false;
            NaRegion.Nodes[3].Online = false;
            Console.WriteLine("done.");
            Console.WriteLine();
            Console.Write("Updating sentinel value... ");
            begin = _stopwatch.Elapsed;
            await NaClient.WriteValueAsync(_sentinelKey, "After");
            end = _stopwatch.Elapsed;
            clientWriteDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine($"Wrote sentinel value in {clientWriteDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            // Wait for writes to complete globally.
            Console.Write("Waiting for write to complete globally... ");
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine("done.");
            Console.WriteLine();
            // Examine value of sentinel key in global nodes while two regional nodes are offline.
            foreach (var node in GlobalNodes)
            {
                Console.WriteLine($"{node.Name} node's {_sentinelKey} = {node.GetValue(_sentinelKey)}.");
            }
            Console.WriteLine();
            // Bring two offline nodes back online.
            NaRegion.Nodes[2].Online = true;
            NaRegion.Nodes[3].Online = true;
            Console.Write("Triggering read repairs by reading sentinel value again... ");
            begin = _stopwatch.Elapsed;
            sentinelValue = await NaClient.ReadValueAsync(_sentinelKey);
            end = _stopwatch.Elapsed;
            clientReadDuration = end - begin;
            Console.WriteLine("done.");
            Console.WriteLine("Read is done.  However, read repairs are in progress.");
            Console.WriteLine($"Sentinel value = {sentinelValue}.");
            Console.WriteLine($"Read sentinel value in {clientReadDuration.TotalSeconds.ToString(_timeSpanFormat)} seconds.");
            Console.WriteLine();
            // Examine value of sentinel key in global nodes.  All nodes are back online and read repairs are in progress.
            var nodeValues = new HashSet<string>();
            foreach (var node in GlobalNodes)
            {
                var value = node.GetValue(_sentinelKey);
                if (!nodeValues.Contains(value)) nodeValues.Add(value);
                Console.WriteLine($"{node.Name} node's {_sentinelKey} = {value}.");
            }
            Console.WriteLine();
            var consistentDuringReadRepairs = nodeValues.Count == 1;
            // Wait for read repairs to complete.
            Console.Write("Waiting for read repairs to complete... ");
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine("done.");
            Console.WriteLine();
            // Examine value of sentinel key in global nodes.  All nodes are back online and read repairs are complete.
            nodeValues.Clear();
            foreach (var node in GlobalNodes)
            {
                var value = node.GetValue(_sentinelKey);
                if (!nodeValues.Contains(value)) nodeValues.Add(value);
                Console.WriteLine($"{node.Name} node's {_sentinelKey} = {value}.");
            }
            Console.WriteLine();
            var consistentAfterReadRepairs = nodeValues.Count == 1;
            var success = !consistentDuringReadRepairs && consistentAfterReadRepairs;
            Console.Write("Test result: ");
            if (success) ThreadsafeConsole.WriteLine("success.", ConsoleColor.Green);
            else ThreadsafeConsole.WriteLine("failure.", ConsoleColor.Red);
        }
    }
}
