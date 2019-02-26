using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.Dapper.Service
{
    public interface IAppSettings
    {
        string Database { get; [UsedImplicitly] set; }
    }
}
