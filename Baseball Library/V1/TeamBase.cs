//using System;
//using System.Collections.Generic;
//using System.Linq;


//namespace ErikTheCoder.Sandbox.Baseball.Library.V1
//{
//    internal abstract class TeamBase : ITeam
//    {
//        private bool _adjustSalaries = true;
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public ICoach HeadCoach { get; set; }
//        public List<ICoach> AssistantCoaches { get; set; }
//        public List<IPlayer> Players { get; set; }


//        protected TeamBase()
//        {
//            AssistantCoaches = new List<ICoach>();
//            Players = new List<IPlayer>();
//        }


//        public IEnumerable<ITeamMember> GetAllTeamMembers()
//        {
//            if (HeadCoach != null) yield return HeadCoach;
//            foreach (var assistantCoach in AssistantCoaches) yield return assistantCoach;
//            foreach (var player in Players) yield return player;
//        }


//        public void Load() => ReadInternal();


//        public void Save()
//        {
//            if (Id == 0) CreateInternal();
//            else UpdateInternal();
//        }


//        public void Delete() => DeleteInternal();


//        // CRUD methods that must be implemented by inheritors.
//        protected abstract void CreateInternal();
//        protected abstract void ReadInternal();
//        protected abstract void UpdateInternal();
//        protected abstract void DeleteInternal();


//        public void AdjustSalaries(decimal SalaryCap)
//        {
//            // Prevent infinite recursion.
//            if (!_adjustSalaries) return;
//            try
//            {
//                _adjustSalaries = false;
//                // Reduce the salary of lowest paid player(s) until team is under cap.
//                List<ITeamMember> teamMembers = GetAllTeamMembers().ToList();
//                do
//                {
//                    var totalSalaries = teamMembers.Sum(TeamMember => TeamMember?.Salary ?? 0);
//                    if (totalSalaries <= SalaryCap) return;
//                    var requiredReduction = totalSalaries - SalaryCap;
//                    var lowestPaidPlayer = GetLowestPaidTeamMember(teamMembers);
//                    lowestPaidPlayer.Salary -= Math.Min(lowestPaidPlayer.Salary, requiredReduction); // Player's salary cannot be reduced below zero.
//                } while (true);
//            }
//            finally
//            {
//                _adjustSalaries = true;
//            }
//        }


//        private static ITeamMember GetLowestPaidTeamMember(List<ITeamMember> TeamMembers)
//        {
//            // Get lowest paid team member that actually earns a salary.
//            TeamMembers.Sort((TeamMember1, TeamMember2) => TeamMember1.Salary.CompareTo(TeamMember2.Salary));
//            foreach (var teamMember in TeamMembers) if (teamMember.Salary > 0) return teamMember;
//            throw new Exception("No team member earns a salary.");
//        }
//    }
//}
