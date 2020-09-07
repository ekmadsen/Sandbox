using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public interface IPipeline
    {
        public Task<long[][]> Run(IMathService MathService, long[] InputValues, long[] StepValues);
    }
}
