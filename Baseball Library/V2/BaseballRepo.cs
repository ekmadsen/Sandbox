namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    public class BaseballRepo : IBaseballRepoRecordPattern
    {
        public static bool UseFile = true;


        public ITeam CreateTeam() => UseFile ? (ITeam)new TeamFile() : new TeamSql(this);
        public ICoach CreateCoach() => new Coach();
        public IPlayers CreatePlayers() => new Players();
        public IPlayer CreatePlayer() => new Player();
        IPlayer IBaseballRepoRecordPattern.CreatePlayer(PlayerRecord Record) => new Player(Record);


        public ITeam GetTeam(int Id)
        {
            var team = CreateTeam();
            team.Id = Id;
            team.Load();
            return team;
        }
    }


    internal interface IBaseballRepoRecordPattern : IBaseballRepo
    {
        IPlayer CreatePlayer(PlayerRecord Record);
    }
}