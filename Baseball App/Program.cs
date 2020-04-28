using System;
using System.Collections.Generic;
using System.Linq;
using ErikTheCoder.Sandbox.Baseball.Library;
using ErikTheCoder.Sandbox.Baseball.Library.V2;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.Baseball.App
{
    public static class Program
    {
        private const int _namePadding = 20;
        private const int _salaryPadding = 15;


        public static void Main(string[] Arguments)
        {
            try
            {
                Console.WriteLine();
                if (Arguments?.Length != 1) throw new ArgumentException("Test name not specified.");
                Run(Arguments);
            }
            catch (Exception exception)
            {
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
            finally
            {
                Console.WriteLine();
            }
        }


        private static void Run(IReadOnlyList<string> Arguments)
        {
            // Run test.
            string testName = Arguments[0];
            switch (testName?.ToLower())
            {
                case "testsalaries":
                    TestSalaries();
                    break;
                case "testsalaryupdate_23_7":
                    TestSalaryUpdate_23_7();
                    break;
                case "testsalaryupdate_7_23":
                    TestSalaryUpdate_7_23();
                    break;
                case "testfileserialization":
                    TestFileSerialization();
                    break;
                case "testsqlserialization":
                    TestSqlSerialization();
                    break;
                default:
                    throw new ArgumentException($"{testName} test not supported.");
            }
        }


        private static void TestSalaries()
        {
            IBaseballRepo repo = new BaseballRepo();
            ITeam cubs1984Team = repo.CreateTeam();
            PopulateCubs1984Team(repo, cubs1984Team);
            PrintSalaries(cubs1984Team);
        }


        private static void TestSalaryUpdate_23_7()
        {
            IBaseballRepo repo = new BaseballRepo();
            ITeam cubs1984Team = repo.CreateTeam();
            PopulateCubs1984Team(repo, cubs1984Team);
            PrintSalaries(cubs1984Team);
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            const decimal sandbergIncrease = 7_000_000m;
            const decimal davisIncrease = 2_000_000m;
            // ReSharper disable PossibleNullReferenceException
            cubs1984Team.Players.First(Player => Player.JerseyNumber == 23).Salary += sandbergIncrease;
            cubs1984Team.Players.First(Player => Player.JerseyNumber == 07).Salary += davisIncrease;
            ThreadsafeConsole.WriteLine($"Increase Sandberg's salary by {sandbergIncrease:C0}");
            ThreadsafeConsole.WriteLine($"Increase Davis'     salary by {davisIncrease:C0}");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            // ReSharper restore PossibleNullReferenceException
            PrintSalaries(cubs1984Team);
        }


        private static void TestSalaryUpdate_7_23()
        {
            IBaseballRepo repo = new BaseballRepo();
            ITeam cubs1984Team = repo.CreateTeam();
            PopulateCubs1984Team(repo, cubs1984Team);
            PrintSalaries(cubs1984Team);
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            const decimal sandbergIncrease = 7_000_000m;
            const decimal davisIncrease = 2_000_000m;
            // ReSharper disable PossibleNullReferenceException
            cubs1984Team.Players.First(Player => Player.JerseyNumber == 07).Salary += davisIncrease;
            cubs1984Team.Players.First(Player => Player.JerseyNumber == 23).Salary += sandbergIncrease;
            ThreadsafeConsole.WriteLine($"Increase Davis'     salary by {davisIncrease:C0}");
            ThreadsafeConsole.WriteLine($"Increase Sandberg's salary by {sandbergIncrease:C0}");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            // ReSharper restore PossibleNullReferenceException
            PrintSalaries(cubs1984Team);
        }


        private static void TestFileSerialization()
        {
            BaseballRepo.UseFile = true;
            IBaseballRepo repo = new BaseballRepo();
            ITeam cubs1984Team = repo.CreateTeam();
            PopulateCubs1984Team(repo, cubs1984Team);
            PrintSalaries(cubs1984Team);
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            cubs1984Team.Save();
            ThreadsafeConsole.WriteLine("Saving team to JSON text file.");
            ThreadsafeConsole.WriteLine("Loading team from JSON text file.");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            cubs1984Team = repo.GetTeam(1);
            PrintSalaries(cubs1984Team);
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine("Saving team to JSON text file.");
            LeagueRegulations.TeamSalaryCap = 40_000_000m;
            ThreadsafeConsole.WriteLine($"Changing team salary cap to {LeagueRegulations.TeamSalaryCap:C0}.");
            ThreadsafeConsole.WriteLine("Loading team from JSON text file.");
            ThreadsafeConsole.WriteLine();
            ThreadsafeConsole.WriteLine();
            cubs1984Team = repo.GetTeam(1);
            PrintSalaries(cubs1984Team);
        }


        private static void TestSqlSerialization()
        {
            BaseballRepo.UseFile = false;
            LeagueRegulations.TeamSalaryCap = 40_000_000m;
            ThreadsafeConsole.WriteLine($"Changing team salary cap to {LeagueRegulations.TeamSalaryCap:C0}.");
            ThreadsafeConsole.WriteLine("Loading team from database.");
            IBaseballRepo repo = new BaseballRepo();
            ITeam cubs1984Team = repo.GetTeam(1);
            PrintSalaries(cubs1984Team);
        }


        private static void PrintSalaries(ITeam Team)
        {
            // Sort by salary descending.
            List<ITeamMember> teamMembers = Team.GetAllTeamMembers().ToList();
            ThreadsafeConsole.WriteLine($"Team Salary Cap = {LeagueRegulations.TeamSalaryCap:C0}.");
            ThreadsafeConsole.WriteLine();
            teamMembers.Sort((TeamMember1, TeamMember2) => TeamMember2.Salary.CompareTo(TeamMember1.Salary));
            const string columnSpacer = "     ";
            ThreadsafeConsole.WriteLine($"{"Team Member", -_namePadding}{columnSpacer}{"Salary", _salaryPadding}");
            ThreadsafeConsole.WriteLine(new string('=', _namePadding + _salaryPadding + columnSpacer.Length));
            foreach (ITeamMember teamMember in teamMembers) ThreadsafeConsole.WriteLine($"{teamMember.Name, -_namePadding}{teamMember.Salary, _namePadding:C0}");
            decimal totalSalaries = teamMembers.Sum(TeamMember => TeamMember.Salary);
            ThreadsafeConsole.WriteLine(new string('=', _namePadding + _salaryPadding + columnSpacer.Length));
            ThreadsafeConsole.WriteLine($"{"Total", -_namePadding}{totalSalaries, _namePadding:C0}");
        }


        private static void PopulateCubs1984Team(IBaseballRepo Repo, ITeam Team)
        {
            Team.Name = "1984 Chicago Cubs";

            // Add players.
            IPlayer player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Rick Sutcliffe";
            player.Team = Team;
            player.Salary = 8_000_000m;
            player.Position = EPosition.Pitcher;
            player.JerseyNumber = 40;
            player.Bats = EHanded.Left;
            player.Throws = EHanded.Right;
            
            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Jody Davis";
            player.Team = Team;
            player.Salary = 1_000_000m;
            player.Position = EPosition.Catcher;
            player.JerseyNumber = 7;
            player.Bats = EHanded.Right;
            player.Throws = EHanded.Right;
            
            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Leon Durham";
            player.Team = Team;
            player.Salary = 6_000_000m;
            player.Position = EPosition.FirstBase;
            player.JerseyNumber = 10;
            player.Bats = EHanded.Left;
            player.Throws = EHanded.Left;

            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Ryne Sandberg";
            player.Team = Team;
            player.Salary = 4_000_000m;
            player.Position = EPosition.SecondBase;
            player.JerseyNumber = 23;
            player.Bats = EHanded.Right;
            player.Throws = EHanded.Right;

            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Ron Cey";
            player.Team = Team;
            player.Salary = 3_000_000m;
            player.Position = EPosition.ThirdBase;
            player.JerseyNumber = 11;
            player.Bats = EHanded.Right;
            player.Throws = EHanded.Right;

            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Larry Bowa";
            player.Team = Team;
            player.Salary = 4_000_000m;
            player.Position = EPosition.Shortstop;
            player.JerseyNumber = 1;
            player.Bats = EHanded.Switch;
            player.Throws = EHanded.Right;

            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Gary Matthews";
            player.Team = Team;
            player.Salary = 4_000_000m;
            player.Position = EPosition.LeftField;
            player.JerseyNumber = 36;
            player.Bats = EHanded.Right;
            player.Throws = EHanded.Right;

            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Bobby Dernier";
            player.Team = Team;
            player.Salary = 3_000_000m;
            player.Position = EPosition.CenterField;
            player.JerseyNumber = 20;
            player.Bats = EHanded.Right;
            player.Throws = EHanded.Right;

            player = Repo.CreatePlayer();
            Team.Players.Add(player);
            player.Name = "Keith Moreland";
            player.Team = Team;
            player.Salary = 3_000_000m;
            player.Position = EPosition.RightField;
            player.JerseyNumber = 6;
            player.Bats = EHanded.Right;
            player.Throws = EHanded.Right;

            // Add coaches.
            ICoach headCoach = Repo.CreateCoach();
            Team.HeadCoach = headCoach;
            headCoach.Name = "Jim Frey";
            headCoach.Team = Team;
            headCoach.Salary = 4_000_000m;
            headCoach.Specialty = "Head Coach";
            headCoach.Manager = null;
            headCoach.Players = Team.Players;

            ICoach coach = Repo.CreateCoach();
            Team.AssistantCoaches.Add(coach);
            coach.Name = "Don Zimmer";
            coach.Team = Team;
            coach.Salary = 2_000_000m;
            coach.Specialty = "Old School Badass";
            coach.Manager = headCoach;
            coach.Players = Repo.CreatePlayers();
            coach.Players.Add(Team.Players[1]);
            coach.Players.Add(Team.Players[2]);
            coach.Players.Add(Team.Players[3]);
            coach.Players.Add(Team.Players[4]);
            coach.Players.Add(Team.Players[5]);
            coach.Players.Add(Team.Players[6]);
            coach.Players.Add(Team.Players[7]);
            coach.Players.Add(Team.Players[8]);
            
            coach = Repo.CreateCoach();
            Team.AssistantCoaches.Add(coach);
            coach.Name = "Billy Connors";
            coach.Team = Team;
            coach.Salary = 2_000_000m;
            coach.Specialty = "Pitching Coach";
            coach.Manager = headCoach;
            coach.Players = Repo.CreatePlayers();
            coach.Players.Add(Team.Players[0]);
        }
    }
}
