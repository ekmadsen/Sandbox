// ReSharper disable UnusedMemberInSuper.Global
using System;
using System.Collections.Generic;
using System.Linq;


namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal abstract class TeamBase : ITeam, ITeamRecordPattern
    {
        private bool _adjustSalaries = true;
        private ICoach _headCoach;
        private ICoaches _assistantCoaches;
        private IPlayers _players;


        public TeamRecord Record { get; set; }


        public int Id
        {
            get => Record.Id;
            set => Record.Id = value;
        }


        public string Name
        {
            get => Record.Name;
            set => Record.Name = value;
        }


        public ICoach HeadCoach
        {
            get => _headCoach;
            set
            {
                _headCoach = value;
                if (value is ICoachRecordPattern pattern) Record.HeadCoach = pattern.Record;
            }
        }


        public ICoaches AssistantCoaches
        {
            get => _assistantCoaches;
            set
            {
                _assistantCoaches = value;
                Record.AssistantCoaches.Clear();
                foreach (var coach in value) if (coach is ICoachRecordPattern pattern) Record.AssistantCoaches.Add(pattern.Record);
            }
        }


        public IPlayers Players
        {
            get => _players;
            set
            {
                _players = value;
                Record.Players.Clear();
                foreach (var player in value) if (player is IPlayerRecordPattern pattern) Record.Players.Add(pattern.Record);
            }
        }


        protected TeamBase(TeamRecord Record)
        {
            this.Record = Record;
            Initialize();
        }


        protected void Initialize()
        {
            if (Record.HeadCoach != null) HeadCoach = new Coach(Record.HeadCoach);
            AssistantCoaches = new Coaches(Record.AssistantCoaches);
            Players = new Players(Record.Players);
        }


        public IEnumerable<ITeamMember> GetAllTeamMembers()
        {
            if (HeadCoach != null) yield return HeadCoach;
            foreach (var assistantCoach in AssistantCoaches) yield return assistantCoach;
            foreach (var player in Players) yield return player;
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
                var teamMembers = GetAllTeamMembers().ToList();
                do
                {
                    var totalSalaries = teamMembers.Sum(TeamMember => TeamMember?.Salary ?? 0);
                    if (totalSalaries <= SalaryCap) return;
                    var requiredReduction = totalSalaries - SalaryCap;
                    var lowestPaidPlayer = GetLowestPaidTeamMember(teamMembers);
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
            foreach (var teamMember in TeamMembers) if (teamMember.Salary > 0) return teamMember;
            throw new Exception("No team member earns a salary.");
        }
    }


    internal interface ITeamRecordPattern
    {
        TeamRecord Record { get; set; }
    }
}
