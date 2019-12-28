using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public abstract class NodeBase
    {
        protected IThreadsafeRandom Random { get; }
        protected ConcurrentDictionary<string, string> Values { get; }
        public int Id { get; }
        public string Name { get; }
        // ReSharper disable once MemberCanBeProtected.Global
        public string RegionName { get; }
        public Dictionary<string, List<Connection>> Connections { get; private set; }
        public bool Online { get; set; }


        protected NodeBase(IThreadsafeRandom Random, int Id, string Name, string RegionName)
        {
            this.Random = Random;
            this.Id = Id;
            this.Name = Name;
            this.RegionName = RegionName;
            Values = new ConcurrentDictionary<string, string>();
            Online = true;
        }


        public void Connect(IEnumerable<NodeBase> Nodes, (TimeSpan MinMs, TimeSpan MaxMs)? Latency = null)
        {
            // Create connections to other nodes.
            Connections = new Dictionary<string, List<Connection>>();
            foreach (NodeBase node in Nodes)
            {
                if (node.Id == Id) continue; // Don't connect node to itself.
                if (!Connections.ContainsKey(node.RegionName)) Connections.Add(node.RegionName, new List<Connection>());
                (TimeSpan minLatency, TimeSpan maxLatency) = Latency ?? LeaderlessReplication.Latency.Get(Id, node.Id);
                Connection connection = new Connection(Random, this, node, minLatency, maxLatency);
                Connections[node.RegionName].Add(connection);
            }
        }


        public abstract Task<string> ReadValueAsync(string Key);
        public string GetValue(string Key) => Values[Key];
        public abstract Task WriteValueAsync(string Key, string Value);
        public void PutValue(string Key, string Value) => Values[Key] = Value;
    }
}
