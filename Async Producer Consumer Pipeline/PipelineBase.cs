using System.Text;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public abstract class PipelineBase : IPipeline
    {
        public abstract Task<long[][]> Run(IMathService MathService, long[] InputValues, long[] StepValues);


        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        protected static void DisplayValues(long[] InputValues, long[] StepValues, long[][] OutputValues)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Step Values: ");
            foreach (var stepValue in StepValues) stringBuilder.Append($"{stepValue,2} ");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("InputValue      Step 1 (Power)      Step 2 (Add)  Step 3 (Multiply)    Step 4 (Modulo)");
            for (var index = 0; index < OutputValues.Length; index++)
            {
                var inputValue = InputValues[index];
                stringBuilder.Append($"{inputValue,10}");
                var stepValues = OutputValues[index];
                foreach (var stepValue in stepValues) stringBuilder.Append($"{stepValue,19:N0}");
                stringBuilder.AppendLine();
            }
            ThreadsafeConsole.WriteLine(stringBuilder.ToString());
        }
    }
}
