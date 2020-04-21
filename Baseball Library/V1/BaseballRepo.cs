namespace ErikTheCoder.Sandbox.Baseball.Library.V1
{
    public class BaseballRepo : IBaseballRepo
    {
        public ITeam CreateTeam() => new TeamFile();
        public ICoach CreateCoach() => new Coach();
        public IPlayer CreatePlayer() => new Player();


        public ITeam GetTeam(int Id)
        {
            TeamFile team = new TeamFile {Id = Id};
            team.Load();
            return team;
        }
    }
}
