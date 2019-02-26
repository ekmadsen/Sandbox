using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.Dapper.Service
{
    [UsedImplicitly]
    public class AppSettings : IAppSettings
    {
        public string Database { get; set; }
    }
}
