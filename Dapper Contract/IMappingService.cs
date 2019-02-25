using System.Threading.Tasks;
using Refit;


namespace ErikTheCoder.Sandbox.Dapper.Contract
{
    public interface IMappingService
    {
        [Get("/mapping/getopenservicecalls")]
        Task<GetOpenServiceCallsResponse> GetOpenServiceCallsAsync();
    }
}
