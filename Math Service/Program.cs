using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


namespace ErikTheCoder.Sandbox.Math.Service
{
    public static class Program
    {
        public static void Main(string[] Args)
        {
            // Build and run web host.
            var webHostBuilder = WebHost.CreateDefaultBuilder(Args);
            webHostBuilder.UseStartup<Startup>();
            var webHost = webHostBuilder.Build();
            webHost.Run();
        }
    }
}
