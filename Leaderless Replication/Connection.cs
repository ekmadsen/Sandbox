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
        public string Name => $"{FromNode?.Name} to {ToNode?.Name}";

        
        public Connection(IThreadsafeRandom Random, NodeBase FromNode, NodeBase ToNode, TimeSpan MinLatency, TimeSpan MaxLatency)
        {
            _random = Random;
            this.FromNode = FromNode;
            this.ToNode = ToNode;
            _minLatency = MinLatency;
            _maxLatency = MaxLatency;
        }


        public async Task<string> ReadValueAsync(string Key)
        {
            await Delay();
            if (!ToNode.Online) throw new NetworkException();
            return await ToNode.ReadValueAsync(Key);
        }


        public async Task<string> GetValueAsync(string Key)
        {
            await Delay();
            if (!ToNode.Online) throw new NetworkException();
            return ToNode.GetValue(Key);
        }


        public async Task WriteValueAsync(string Key, string Value)
        {
            await Delay();
            if (!ToNode.Online) throw new NetworkException();
            await ToNode.WriteValueAsync(Key, Value);
        }


        public async Task PutValueAsync(string Key, string Value)
        {
            await Delay();
            if (!ToNode.Online) throw new NetworkException();
            ToNode.PutValue(Key, Value);
        }


        private async Task Delay()
        {
            TimeSpan latency = TimeSpan.FromMilliseconds(_random.NextDouble(_minLatency.TotalMilliseconds, _maxLatency.TotalMilliseconds));
            await Task.Delay(latency);
        }
    }
}
