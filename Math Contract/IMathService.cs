using System.Threading.Tasks;
using Refit;


namespace ErikTheCoder.Sandbox.Math.Contract
{
    public interface IMathService
    {
        [Get("/math/factorial")]
        Task<int> Factorial(int InputValue);


        [Get("/math/add")]
        Task<int> Add(int InputValue, int Value);


        [Get("/math/multiply")]
        Task<int> Multiply(int InputValue, int Value);        


        [Get("/math/modulo")]
        Task<int> Modulo(int InputValue, int Value);
    }
}