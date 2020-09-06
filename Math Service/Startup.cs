using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace ErikTheCoder.Sandbox.Math.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection Services)
        {
            Services.AddMvc();
            Services.AddRouting();
        }

        
        public void Configure(IApplicationBuilder ApplicationBuilder)
        {
            ApplicationBuilder.UseRouting();
            ApplicationBuilder.UseEndpoints(RouteBuilder =>
            {
                RouteBuilder.MapControllerRoute("default", "{controller}/{action}");
            });
        }
    }
}
