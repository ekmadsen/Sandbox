using System;
using System.Threading.Tasks;
using ErikTheCoder.Utilities;

namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public class Connection
    {
        private readonly IThreadsafeRandom _random;
        private readonly TimeSpan _minLatency;
        private readonly TimeSpan _maxLatency;


        public NodeBase FromNode { get; }
        public NodeBase ToNode { get; }
        public bool Online { get; set; }


        public Connection(IThreadsafeRandom Random, NodeBase FromNode, NodeBase ToNode, TimeSpan MinLatency, TimeSpan MaxLatency)
        {
            _random = Random;
            this.FromNode = FromNode;
            this.ToNode = ToNode;
            _minLatency = MinLatency;
            _maxLatency = MaxLatency;
            Online = true;
        }


        public async Task<string> ReadValueAsync(int Key)
        {
            await Delay();
            if (!Online) throw new NetworkException();
            return await ToNode.ReadValueAsync(Key);
        }


        public async Task WriteValueAsync(int Key, string Value)
        {
            await Delay();
            if (!Online) throw new NetworkException();
            // Calling ToNode.WriteValueAsync would create an infinite loop of writes between all nodes.
            ToNode.ReceiveValue(Key, Value);
        }


        private async Task Delay()
        {
            TimeSpan latency = TimeSpan.FromMilliseconds(_random.NextDouble(_minLatency.TotalMilliseconds, _maxLatency.TotalMilliseconds));
            await Task.Delay(latency);
        }
    }
}
