using System;
using System.Collections.Generic;
using System.Data;
using Moq;


namespace ErikTheCoder.Sandbox.Baseball.Library.V1
{
    internal class TeamSql : TeamBase
    {
        private readonly IBaseballRepo _repo;


        public TeamSql(IBaseballRepo Repo)
        {
            _repo = Repo;
        }


        protected override void CreateInternal() => throw new System.NotImplementedException();
        protected override void UpdateInternal() => throw new System.NotImplementedException();
        protected override void DeleteInternal() => throw new System.NotImplementedException();


        protected override void ReadInternal()
        {
            // Mock reading a team's members from a database.
            const string sql = @"
                select p.*
                from tblPlayers p
                where p.TeamId = @TeamId
                union all
                select c.*
                from tblCoaches c
                where c.TeamId = @TeamId
                ";
            IDataReader dataReader = GetDataReader(sql);
            while (dataReader.Read())
            {
                // Add everyone to the players list.
                // The point of this code is not to implement a working solution.  The point is to demonstrate the player salary side effect.
                IPlayer player = _repo.CreatePlayer();
                Players.Add(player);
                player.Team = this;
                player.Name = dataReader["Name"]?.ToString();
                player.Salary = decimal.Parse(dataReader["Salary"]?.ToString() ?? "0");
                
            }
        }

        private IDataReader GetDataReader(string Sql)
        {
            Mock<IDataReader> moqDataReader = new Mock<IDataReader>();
            List<Dictionary<string, object>> data = GetData();
            int rowNumber = -1;
            int maxRowNumber = data.Count - 2; // Unsure why this isn't data.Count - 1?
            moqDataReader.Setup(Instance => Instance.Read())
                .Returns(() => rowNumber <= maxRowNumber)
                .Callback(() => rowNumber++);
            moqDataReader.Setup(Instance => Instance["Name"])
                .Returns(() => data[rowNumber]["Name"]);
            moqDataReader.Setup(Instance => Instance["Salary"])
                .Returns(() => data[rowNumber]["Salary"]);
            return moqDataReader.Object;
        }


        private static List<Dictionary<string, object>> GetData()
        {
            return new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"Name", "Rick Sutcliffe"},
                    {"Salary", 8_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Jody Davis"},
                    {"Salary", 1_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Leon Durham"},
                    {"Salary", 6_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Ryne Sandberg"},
                    {"Salary", 4_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Ron Cey"},
                    {"Salary", 3_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Larry Bowa"},
                    {"Salary", 4_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Gary Matthews"},
                    {"Salary", 4_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Bobby Dernier"},
                    {"Salary", 3_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Keith Moreland"},
                    {"Salary", 3_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Jim Frey"},
                    {"Salary", 4_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Don Zimmer"},
                    {"Salary", 2_000_000m}
                },
                new Dictionary<string, object>()
                {
                    {"Name", "Billy Connors"},
                    {"Salary", 2_000_000m}
                }
            };
        }
    }
}
