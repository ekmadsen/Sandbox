using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErikTheCoder.Utilities;

namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public class QuorumNode : NodeBase
    {
        private readonly int _requiredVotes;


        public QuorumNode(IThreadsafeRandom Random, int Id, string Name, string RegionName, List<NodeBase> ConnectedNodes, int RequiredVotes) :
            base(Random, Id, Name, RegionName, ConnectedNodes)
        {
            _requiredVotes = RequiredVotes;
        }


        public override async Task<string> ReadValueAsync(int Key)
        {
            // Read value from connected nodes in same region.
            Dictionary<string, int> allVotes = new Dictionary<string, int>();
            List<Task<string>> regionTasks = new List<Task<string>>();
            foreach (Connection connection in Connections[RegionName]) regionTasks.Add(connection.ReadValueAsync(Key));
            int remainingTasks = regionTasks.Count;
            while (remainingTasks > 0)
            {
                var task = await Task.WhenAny(regionTasks);
                remainingTasks--;
                string value;
                try
                {
                    value = await task;
                }
                catch (NetworkException)
                {
                    // Ignore offline nodes.
                    continue;
                }
                // Tally votes for each read value and return when quorum is reached.
                if (!allVotes.TryGetValue(value, out int votes)) votes = 0;
                allVotes[value] = ++votes;
                if (votes >= _requiredVotes) return value;
            }
            throw new Exception("Quorum not reached.");
        }


        public override async Task WriteValueAsync(int Key, string Value)
        {
            // Write value to local collection.
            Values[Key] = Value;
            // Write value to all connected nodes in same region.
            List<Task> regionTasks = new List<Task>();
            foreach (Connection connection in Connections[RegionName]) regionTasks.Add(connection.WriteValueAsync(Key, Value));
            // Write value to all other connected nodes.
            foreach ((string regionName, List<Connection> connections) in Connections)
            {
                if (regionName == RegionName) continue; // Already have written value to nodes in same region.
                foreach (Connection connection in connections) _ = connection.WriteValueAsync(Key, Value);
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
