namespace ErikTheCoder.Sandbox.Baseball.Library
{
    public interface ITeamMember
    {
        string Name { get; set; }
        decimal Salary { get; set; }
        ITeam Team { get; set; }
    }
}
