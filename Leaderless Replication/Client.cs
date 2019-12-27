using System;
using System.Threading.Tasks;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public class Client : NodeBase
    {
        public Client(IThreadsafeRandom Random, int Id, string Name, string RegionName) :
            base(Random, Id, Name, RegionName)
        {
        }


        public override async Task<string> ReadValueAsync(string Key)
        {
            // Load-balance read requests.
            // Connect to nodes in same region only.
            ShuffleConnections();
            foreach (Connection connection in Connections[RegionName])
            {
                try
                {
                    return await connection.ReadValueAsync(Key);
                }
                catch (NetworkException)
                {
                    // Ignore offline node.
                }
            }
            throw new Exception("Read failed.");
        }


        public override async Task WriteValueAsync(string Key, string Value)
        {
            // Load-balance write requests.
            // Connect to nodes in same region only.
            // Rely on randomly selected regional node to push writes to all global nodes.
            ShuffleConnections();
            foreach (Connection connection in Connections[RegionName])
            {
                try
                {
                    await connection.WriteValueAsync(Key, Value);
                    return;
                }
                catch (NetworkException)
                {
                    // Ignore offline node.
                }
            }
            throw new Exception("Write failed.");
        }


        // Not thread-safe.  Assume client can make only one (read or write) concurrent request.
        private void ShuffleConnections() => Connections[RegionName].Shuffle(Random);
    }
}
