namespace ErikTheCoder.Sandbox.Baseball.Library.V1
{
    public class BaseballRepo : IBaseballRepo
    {
        public ITeam CreateTeam() => new TeamSql(this);
        public ICoach CreateCoach() => new Coach();
        public IPlayer CreatePlayer() => new Player();


        public ITeam GetTeam(int Id)
        {
            ITeam team = CreateTeam();
            team.Id = Id;
            team.Load();
            return team;
        }
    }
}
