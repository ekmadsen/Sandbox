using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Baseball.Library.V1
{
    internal class Coach : TeamMember, ICoach
    {
        public string Specialty { get; set; }
        public ICoach Manager { get; set; }
        public List<IPlayer> Players { get; set; }
    }
}
