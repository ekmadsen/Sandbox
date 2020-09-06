using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.ServiceProxy;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public class PipelineV1 : PipelineBase
    {
        public override async Task<int[][]> Run(Proxy<IMathService> MathService, int[] InputValues, int[] StepValues)
        {
            // Create output array.
            var outputValues = new int[InputValues.Length][];
            for (var index = 0; index < outputValues.Length; index++)
            {
                outputValues[index] = new int[StepValues.Length];
            }
            // To establish baseline performance, call math service methods sequentially.
            for (var index = 0; index < InputValues.Length; index++)
            {
                var inputValue = InputValues[index];
                // Step 1 : Factorial
                var outputValue = await MathService.AsUser.Factorial(inputValue);
                outputValues[index][0] = outputValue;
                inputValue = outputValue;
                // Step 2 : Add
                var stepValue = StepValues[1];
                outputValue = await MathService.AsUser.Add(inputValue, stepValue);
                outputValues[index][1] = outputValue;
                inputValue = outputValue;
                // Step 3 : Multiply
                stepValue = StepValues[2];
                outputValue = await MathService.AsUser.Multiply(inputValue, stepValue);
                outputValues[index][2] = outputValue;
                inputValue = outputValue;
                // Step 4 : Modulo
                stepValue = StepValues[3];
                outputValue = await MathService.AsUser.Modulo(inputValue, stepValue);
                outputValues[index][3] = outputValue;
                // Display values.
                DisplayValues(InputValues, StepValues, outputValues);
            }
            return outputValues;
        }
    }
}
