using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.LeaderlessReplication
{
    public class Region
    {
        public string Name { get; }
        public List<NodeBase> Nodes { get; }


        public Region(string Name, List<NodeBase> Nodes)
        {
            this.Name = Name;
            this.Nodes = Nodes;
        }
    }
}
