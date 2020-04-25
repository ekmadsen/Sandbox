using System;


namespace ErikTheCoder.Sandbox.Baseball.Library.V1
{
    internal abstract class TeamMemberBase : ITeamMember
    {
        private decimal _salary;


        public string Name { get; set; }
        public ITeam Team { get; set; }


        public decimal Salary
        {
            get => _salary;
            set
            {
                if ((value < 0) || (value > LeagueRegulations.TeamSalaryCap)) throw new ArgumentOutOfRangeException();
                bool salaryIncrease = value > _salary;
                _salary = value;
                if (salaryIncrease) Team.AdjustSalaries(LeagueRegulations.TeamSalaryCap); // Enforce league's salary cap.
            }
        }
    }
}
