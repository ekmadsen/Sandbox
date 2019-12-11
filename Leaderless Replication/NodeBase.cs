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
        protected ConcurrentDictionary<int, string> Values { get; }
        public int Id { get; }
        public string Name { get; }
        public string RegionName { get; }
        public Dictionary<string, List<Connection>> Connections { get; }
        public bool Online { get; set; }


        protected NodeBase(IThreadsafeRandom Random, int Id, string Name, string RegionName, List<NodeBase> ConnectedNodes)
        {
            this.Random = Random;
            this.Id = Id;
            this.Name = Name;
            this.RegionName = RegionName;
            // Create connections to other nodes.
            Connections = new Dictionary<string, List<Connection>>();
            foreach (NodeBase node in ConnectedNodes)
            {
                if (!Connections.ContainsKey(node.RegionName)) Connections.Add(node.RegionName, new List<Connection>());
                (TimeSpan minLatency, TimeSpan maxLatency) = Latency.Get(Id, node.Id);
                Connection connection = new Connection(this.Random, this, node, minLatency, maxLatency);
                Connections[node.RegionName].Add(connection);
            }
            Values = new ConcurrentDictionary<int, string>();
            Online = true;
        }


        public abstract Task<string> ReadValueAsync(int Key);


        public abstract Task WriteValueAsync(int Key, string Value);


        public void ReceiveValue(int Key, string Value) => Values[Key] = Value;
    }
}
