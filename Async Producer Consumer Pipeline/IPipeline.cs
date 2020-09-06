using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.ServiceProxy;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public interface IPipeline
    {
        public Task<int[][]> Run(Proxy<IMathService> MathService, int[] InputValues, int[] StepValues);
    }
}
