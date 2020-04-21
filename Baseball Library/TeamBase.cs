using System;
using System.Collections.Generic;
using System.Linq;


namespace ErikTheCoder.Sandbox.Baseball.Library
{
    internal abstract class TeamBase : ITeam
    {
        private bool _adjustSalaries = true;
        public int Id { get; set; }
        public string Name { get; set; }
        public ICoach HeadCoach { get; set; }
        public List<ICoach> AssistantCoaches { get; set; }
        public List<IPlayer> Players { get; set; }
        

        public IEnumerable<ITeamMember> GetAllTeamMembers()
        {
            yield return HeadCoach;
            foreach (ICoach assistantCoach in AssistantCoaches) yield return assistantCoach;
            foreach (IPlayer player in Players) yield return player;
        }


        public void Load() => ReadInternal();


        public void Save()
        {
            if (Id == 0) CreateInternal();
            else UpdateInternal();
        }


        public void Delete() => DeleteInternal();


        // CRUD methods that must be implemented by inheritors.
        protected abstract void CreateInternal();
        protected abstract void ReadInternal();
        protected abstract void UpdateInternal();
        protected abstract void DeleteInternal();


        public void AdjustSalaries(decimal SalaryCap)
        {
            // Prevent infinite recursion.
            if (!_adjustSalaries) return;
            try
            {
                _adjustSalaries = false;
                // Reduce the salary of lowest paid player(s) until team is under cap.
                List<ITeamMember> teamMembers = GetAllTeamMembers().ToList();
                do
                {
                    decimal totalSalaries = teamMembers.Sum(TeamMember => TeamMember?.Salary ?? 0);
                    if (totalSalaries <= SalaryCap) return;
                    decimal requiredReduction = totalSalaries - SalaryCap;
                    ITeamMember lowestPaidPlayer = GetLowestPaidTeamMember(teamMembers);
                    lowestPaidPlayer.Salary -= Math.Min(lowestPaidPlayer.Salary, requiredReduction); // Player's salary cannot be reduced below zero.
                } while (true);
            }
            finally
            {
                _adjustSalaries = true;
            }
        }


        private static ITeamMember GetLowestPaidTeamMember(List<ITeamMember> TeamMembers)
        {
            // Get lowest paid team member that actually earns a salary.
            TeamMembers.Sort((TeamMember1, TeamMember2) => TeamMember1.Salary.CompareTo(TeamMember2.Salary));
            foreach (ITeamMember teamMember in TeamMembers) if (teamMember.Salary > 0) return teamMember;
            throw new Exception("No team member earns a salary.");
        }
    }
}
