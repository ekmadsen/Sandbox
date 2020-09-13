//namespace ErikTheCoder.Sandbox.Baseball.Library.V1
//{
//    public class BaseballRepo : IBaseballRepo
//    {
//        public static bool UseFile = true;


//        public ITeam CreateTeam() => UseFile ? (ITeam) new TeamFile() : new TeamSql(this);
//        public ICoach CreateCoach() => new Coach();
//        public IPlayer CreatePlayer() => new Player();


//        public ITeam GetTeam(int Id)
//        {
//            var team = CreateTeam();
//            team.Id = Id;
//            team.Load();
//            return team;
//        }
//    }
//}
