using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Baseball.Library
{
    public interface ICoach : ITeamMember
    {
        string Specialty { get; set; }
        ICoach Manager { get; set; } // Reports to this manager.
        List<IPlayer> Players { get; set; } // Responsible for coaching these players.
    }
}
