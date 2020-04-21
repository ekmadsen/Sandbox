namespace ErikTheCoder.Sandbox.Baseball.Library
{
    public interface IBaseballRepo
    {
        ITeam CreateTeam();
        ITeam GetTeam(int Id);
        ICoach CreateCoach();
        IPlayer CreatePlayer();
    }
}
