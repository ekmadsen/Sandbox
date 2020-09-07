using System.Threading.Tasks;
using Refit;


namespace ErikTheCoder.Sandbox.Math.Contract
{
    public interface IMathService
    {
        [Get("/math/power")]
        Task<long> Power(long InputValue, long Value);


        [Get("/math/add")]
        Task<long> Add(long InputValue, long Value);


        [Get("/math/multiply")]
        Task<long> Multiply(long InputValue, long Value);        


        [Get("/math/modulo")]
        Task<long> Modulo(long InputValue, long Value);
    }
}