using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErikTheCoder.Utilities;

namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public class QuorumNode : NodeBase
    {
        private readonly int _requiredVotes;


        public bool ReadRepair { get; set; }


        public QuorumNode(IThreadsafeRandom Random, int Id, string Name, string RegionName, int RequiredVotes) :
            base(Random, Id, Name, RegionName)
        {
            _requiredVotes = RequiredVotes;
        }


        public override async Task<string> ReadValueAsync(string Key)
        {
            // Track which nodes voted for which values.  Initialize with this node's value.
            Dictionary<string, HashSet<int>> allVotes = new Dictionary<string, HashSet<int>> {[Values[Key]] = new HashSet<int> {Id}};
            // Get value from connected nodes in same region.
            // Calling connection.ReadValueAsync would create an infinite loop of reads from all regional nodes to all regional nodes.
            HashSet<Task<string>> regionTasks = new HashSet<Task<string>>();
            foreach (Connection connection in Connections[RegionName]) regionTasks.Add(connection.GetValueAsync(Key));
            while (regionTasks.Count > 0)
            {
                // Tally votes as they arrive.
                Task<string> task = await Task.WhenAny(regionTasks);
                regionTasks.Remove(task);
                string value;
                try
                {
                    value = await task;
                }
                catch (NetworkException)
                {
                    // Ignore offline node.
                    continue;
                }
                // Return when quorum is reached.
                string votesKey = value ?? "null";
                if (!allVotes.TryGetValue(votesKey, out HashSet<int> nodes)) nodes = new HashSet<int>();
                nodes.Add()
                votes++;
                allVotes[votesKey] = votes;
                if (votes >= _requiredVotes) return value;
            }
            throw new QuorumNotReachedException();
        }


        public override async Task WriteValueAsync(string Key, string Value)
        {
            // Put value to local collection.
            // Calling connection.WriteValueAsync would create an infinite loop of writes from all global nodes to all global nodes.
            Values[Key] = Value;
            // Put value to all connected nodes in same region.
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
