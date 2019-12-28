using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public class Region
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string Name { get; }
        public List<NodeBase> Nodes { get; }


        public Region(string Name, List<NodeBase> Nodes)
        {
            this.Name = Name;
            this.Nodes = Nodes;
        }
    }
}
