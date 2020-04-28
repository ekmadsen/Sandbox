using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal class CoachRecord : TeamMemberRecord
    {
        public string Specialty;
        public CoachRecord Manager;
        public List<PlayerRecord> Players;


        public CoachRecord()
        {
            Players = new List<PlayerRecord>();
        }
    }
}
