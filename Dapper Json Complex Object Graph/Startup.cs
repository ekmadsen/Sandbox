using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Dapper.Service
{
    public class Startup
    {
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection Services)
        {
            // Add MVC, filters, policies, and configure routing.
            Services.AddMvc().AddJsonOptions(Options =>
                {
                    // Preserve case of property names.
                    Options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    // Preserve cyclical object references. 
                    Options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All; 
                }
            );
            Services.AddRouting(Options => Options.LowercaseUrls = true);
            // Configure dependency injection.
            Services.AddSingleton(typeof(IAppSettings), Program.AppSettings);
        }


        [UsedImplicitly]
        public void Configure(IApplicationBuilder ApplicationBuilder)
        {
            ApplicationBuilder.UseMvc();
        }
    }
}
