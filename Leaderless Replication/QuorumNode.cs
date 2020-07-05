using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErikTheCoder.Utilities;

namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public class QuorumNode : NodeBase
    {
        private const string _nullValue = "!!null!!";
        private readonly int _requiredVotes;
        private readonly bool _readRepair;


        public QuorumNode(IThreadsafeRandom Random, int Id, string Name, string RegionName, int RequiredVotes, bool ReadRepair) :
            base(Random, Id, Name, RegionName)
        {
            _requiredVotes = RequiredVotes;
            _readRepair = ReadRepair;
        }


        public override async Task<string> ReadValueAsync(string Key)
        {
            // Track which nodes voted for which values.  Initialize with this node's value.
            Dictionary<string, HashSet<int>> votes = new Dictionary<string, HashSet<int>> {[Values[Key]] = new HashSet<int> {Id}};
            // Get value from connected nodes in same region.
            HashSet<Task<(string Value, int ToNodeId)>> regionTasks = new HashSet<Task<(string Value, int ToNodeId)>>();


            //HashSet<Task<(string Value, int ToNodeId)>> regionTasks = Connections[RegionName].Select(async Connection =>
            //{
            //    string value = await Connection.GetValueAsync(Key);
            //    return (value, Connection.ToNode.Id);
            //}).ToHashSet();


            foreach (Connection connection in Connections[RegionName])
            {
                // Fails to compile in C# (but works in VB.NET).
                //regionTasks.Add(async () =>
                //{
                //    // Calling connection.ReadValueAsync would create an infinite loop of reads from all regional nodes to all regional nodes.
                //    string value = await connection.GetValueAsync(Key);
                //    return (value, connection.ToNode.Id);
                //}());


                // Use a local function.
                //async Task<(string Value, int ToNodeId)> GetValueAndToNodeIdAsync(Connection Connection)
                //{
                //    // Calling connection.ReadValueAsync would create an infinite loop of reads from all regional nodes to all regional nodes.
                //    string value = await Connection.GetValueAsync(Key);
                //    return (value, Connection.ToNode.Id);
                //}
                //regionTasks.Add(GetValueAndToNodeIdAsync(connection));


                // Explicitly declare a Func and invoke it immediately.
                //Func<Task<(string Value, int ToNodeId)>> getValueAndToNodeIdAsync = async () =>
                //{
                //    // Calling connection.ReadValueAsync would create an infinite loop of reads from all regional nodes to all regional nodes.
                //    string value = await connection.GetValueAsync(Key);
                //    return (value, connection.ToNode.Id);
                //};
                //regionTasks.Add(getValueAndToNodeIdAsync());


                // Cast the lambda method.
                //regionTasks.Add(((Func<Task<(string Values, int ToNodeId)>>)(async () =>
                //{
                //    // Calling connection.ReadValueAsync would create an infinite loop of reads from all regional nodes to all regional nodes.
                //    string value = await connection.GetValueAsync(Key);
                //    return (value, connection.ToNode.Id);
                //}))());


                // Use a helper method to make the lambda method's return value unambiguous.
                regionTasks.Add(AsyncHelper.MaterializeTask(async () =>
                {
                    // Calling connection.ReadValueAsync would create an infinite loop of reads from all regional nodes to all regional nodes.
                    string value = await connection.GetValueAsync(Key);
                    return (value, connection.ToNode.Id);
                }));
            }


            while (regionTasks.Count > 0)
            {
                // Tally votes as they arrive.
                Task<(string Value, int ToNodeId)> task = await Task.WhenAny(regionTasks);
                regionTasks.Remove(task);
                string value;
                int toNodeId;
                try
                {
                    (value, toNodeId) = await task;
                }
                catch (NetworkException)
                {
                    // Ignore offline node.
                    continue;
                }
                string voteKey = value ?? _nullValue;
                if (!votes.TryGetValue(voteKey, out HashSet<int> nodes)) nodes = new HashSet<int>();
                nodes.Add(toNodeId);
                votes[voteKey] = nodes;
                if (_readRepair)
                {
                    // Return when all votes are tallied.
                    if (regionTasks.Count == 0) return ReadValueAndRepairNodes(Key, votes);
                }
                else 
                {
                    // Return when quorum is reached.
                    if (nodes.Count >= _requiredVotes) return value;
                }
            }
            throw new QuorumNotReachedException();
        }


        private string ReadValueAndRepairNodes(string Key, Dictionary<string, HashSet<int>> Votes)
        {
            // Determine correct value.
            bool quorumReached = false;
            string correctValue = null;
            HashSet<int> correctNodes = null;
            foreach ((string value, HashSet<int> nodes) in Votes)
            {
                if (nodes.Count >= _requiredVotes)
                {
                    quorumReached = true;
                    correctValue = value == _nullValue ? null : value;
                    correctNodes = nodes;
                    break;
                }
            }
            if (!quorumReached) throw new QuorumNotReachedException();
            if (Votes.Count > 1)
            {
                // Repair nodes that have wrong value or were offline.  Don't wait for writes to complete.
                foreach (Connection connection in Connections[RegionName]) if (!correctNodes.Contains(connection.ToNode.Id)) _ = connection.WriteValueAsync(Key, correctValue);
            }
            return correctValue;
        }


        public override async Task WriteValueAsync(string Key, string Value)
        {
            // Put value to local collection.
            Values[Key] = Value;
            // Put value to all connected nodes in same region.
            // Calling connection.WriteValueAsync would create an infinite loop of writes from all global nodes to all global nodes.
            List<Task> regionTasks = new List<Task>();
            foreach (Connection connection in Connections[RegionName]) regionTasks.Add(connection.PutValueAsync(Key, Value));
            // Put value to all other connected nodes.
            foreach ((string regionName, List<Connection> connections) in Connections)
            {
                if (regionName == RegionName) continue; // Already have put value to nodes in same region.
                foreach (Connection connection in connections) _ = connection.PutValueAsync(Key, Value);
            }
            // Wait for nodes in same region to respond.
            try
            {
                await Task.WhenAll(regionTasks);
            }
            catch (NetworkException)
            {
                // Ignore offline nodes.
            }
        }
    }
}
