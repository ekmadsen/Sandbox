using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Baseball.Library
{
    public interface ITeam
    {
        int Id { get; set; }
        string Name { get; set; }
        ICoach HeadCoach { get; set; }
        List<ICoach> AssistantCoaches { get; set; }
        List<IPlayer> Players { get; set; }


        IEnumerable<ITeamMember> GetAllTeamMembers();
        void AdjustSalaries(decimal SalaryCap);
        public void Load();
        public void Save();
        public void Delete();
    }
}
