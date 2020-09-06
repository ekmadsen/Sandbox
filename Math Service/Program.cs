using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


namespace ErikTheCoder.Sandbox.Math.Service
{
    public static class Program
    {
        public static void Main(string[] Args)
        {
            // Build and run web host.
            IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder(Args);
            webHostBuilder.UseStartup<Startup>();
            IWebHost webHost = webHostBuilder.Build();
            webHost.Run();
        }
    }
}
