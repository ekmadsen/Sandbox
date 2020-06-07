// ReSharper disable UnusedMember.Global
namespace ErikTheCoder.Sandbox.Baseball.Library
{
    public interface IPlayer : ITeamMember
    {
        EPosition Position { get; set; }
        int JerseyNumber { get; set; }
        EHanded Bats { get; set; }
        EHanded Throws { get; set; }
    }
}
