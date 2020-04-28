﻿using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Baseball.Library
{
    public interface ITeam
    {
        int Id { get; set; }
        string Name { get; set; }
        ICoach HeadCoach { get; set; }
        ICoaches AssistantCoaches { get; set; }
        IPlayers Players { get; set; }


        IEnumerable<ITeamMember> GetAllTeamMembers();
        void AdjustSalaries(decimal SalaryCap);
        void Load();
        void Save();
        void Delete();
    }
}
