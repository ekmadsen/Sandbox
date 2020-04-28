using System;


namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal abstract class TeamMemberBase : ITeamMember
    {
        private readonly TeamMemberRecord _record;
        private ITeam _team;
        


        public string Name
        {
            get => _record.Name;
            set => _record.Name = value;
        }


        public ITeam Team
        {
            get => _team;
            set
            {
                _team = value;
                if (value is ITeamRecordPattern pattern) _record.Team = pattern.Record;
            }
        }


        public decimal Salary
        {
            get => _record.Salary;
            set
            {
                if ((value < 0) || (value > LeagueRegulations.TeamSalaryCap)) throw new ArgumentOutOfRangeException();
                bool salaryIncrease = value > _record.Salary;
                _record.Salary = value;
                if (salaryIncrease) Team.AdjustSalaries(LeagueRegulations.TeamSalaryCap); // Enforce league's salary cap.
            }
        }


        protected TeamMemberBase(TeamMemberRecord Record)
        {
            _record = Record;
        }
    }
}
