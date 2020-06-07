// ReSharper disable FieldCanBeMadeReadOnly.Global
using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal class TeamRecord
    {
        public int Id;
        public string Name;
        public CoachRecord HeadCoach;
        public List<CoachRecord> AssistantCoaches;
        public List<PlayerRecord> Players;


        public TeamRecord()
        {
            AssistantCoaches = new List<CoachRecord>();
            Players = new List<PlayerRecord>();
        }
    }
}
