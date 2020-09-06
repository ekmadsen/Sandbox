using System.Text;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.ServiceProxy;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public abstract class PipelineBase : IPipeline
    {
        public abstract Task<int[][]> Run(Proxy<IMathService> MathService, int[] InputValues, int[] StepValues);


        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        protected static void DisplayValues(int[] InputValues, int[] StepValues, int[][] OutputValues)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Step Values: ");
            foreach (var stepValue in StepValues) stringBuilder.Append($"{stepValue,2} ");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("InputValue  Step 1 (Factorial)      Step 2 (Add)  Step 3 (Multiply)    Step 4 (Modulo)");
            for (var index = 0; index < OutputValues.Length; index++)
            {
                var inputValue = InputValues[index];
                stringBuilder.Append($"{inputValue,10}");
                var stepValues = OutputValues[index];
                foreach (var stepValue in stepValues) stringBuilder.Append($"{stepValue,19:N0}");
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine();
            ThreadsafeConsole.WriteLine(stringBuilder.ToString());
        }
    }
}
